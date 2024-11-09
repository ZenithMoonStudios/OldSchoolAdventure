using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace EasyStorage
{
    /// <summary>
    /// A base class for an object that maintains a StorageDevice.
    /// </summary>
    /// <remarks>
    /// We implement IGameComponent and IUpdateable rather than deriving from GameComponent
    /// just to simplify our constructor and remove the need to pass the Game to it.
    /// 
    /// The implementation for the synchronous actions of ISaveDevice are found in
    /// SaveDeviceSynchronous.cs.
    /// 
    /// The implementation for the asynchronous actions of IAsyncSaveDevice are found in
    /// SaveDeviceAsync.cs.
    /// </remarks>
    public abstract partial class SaveDevice : IGameComponent, IUpdateable, IAsyncSaveDevice
    {
        #region Static Strings

        // strings for various message boxes
        private static string promptForCancelledMessage;
        private static string forceCancelledReselectionMessage;
        private static string promptForDisconnectedMessage;
        private static string forceDisconnectedReselectionMessage;
        private static string deviceRequiredTitle;
        private static string deviceOptionalTitle;
        private static readonly string[] deviceOptionalOptions = new string[2];
        private static readonly string[] deviceRequiredOptions = new string[1];

        /// <summary>
        /// Gets or sets the message displayed when the user is asked if they want
        /// to select a storage device after cancelling the storage device selector.
        /// </summary>
        public static string PromptForCancelledMessage
        {
            get { return promptForCancelledMessage; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    promptForCancelledMessage = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the message displayed when the user is told they must
        /// select a storage device after cancelling the storage device selector.
        /// </summary>
        public static string ForceCancelledReselectionMessage
        {
            get { return forceCancelledReselectionMessage; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    forceCancelledReselectionMessage = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the message displayed when the user is asked if they
        /// want to select a new storage device after the storage device becomes
        /// disconnected.
        /// </summary>
        public static string PromptForDisconnectedMessage
        {
            get { return promptForDisconnectedMessage; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    promptForDisconnectedMessage = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the message displayed when the user is told they must
        /// select a new storage device after the storage device becomes disconnected.
        /// </summary>
        public static string ForceDisconnectedReselectionMessage
        {
            get { return forceDisconnectedReselectionMessage; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    forceDisconnectedReselectionMessage = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the title displayed when the user is required to choose
        /// a storage device.
        /// </summary>
        public static string DeviceRequiredTitle
        {
            get { return deviceRequiredTitle; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    deviceRequiredTitle = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the title displayed when the user is asked if they want
        /// to choose a storage device.
        /// </summary>
        public static string DeviceOptionalTitle
        {
            get { return deviceOptionalTitle; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    deviceOptionalTitle = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the text used for the "Ok" button when the user is told
        /// they must select a storage device.
        /// </summary>
        public static string OkOption
        {
            get { return deviceRequiredOptions[0]; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    deviceRequiredOptions[0] = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the text used for the "Yes" button when the user is asked
        /// if they want to select a storage device.
        /// </summary>
        public static string YesOption
        {
            get { return deviceOptionalOptions[0]; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    deviceOptionalOptions[0] = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        /// <summary>
        /// Gets or sets the text used for the "No" button when the user is asked
        /// if they want to select a storage device.
        /// </summary>
        public static string NoOption
        {
            get { return deviceOptionalOptions[1]; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    deviceOptionalOptions[1] = value.Length < 256 ? value : value.Substring(0, 256);
            }
        }

        #endregion

        // the update order for the component and its enabled state
        private int updateOrder;
        private bool enabled = true;

        // was the device connected last frame?
        private bool deviceWasConnected;

        // the current state of the SaveDevice
        private SaveDevicePromptState state = SaveDevicePromptState.None;

        // we store the callbacks as fields to reduce run-time allocation and garbage
        private readonly AsyncCallback storageDeviceSelectorCallback;
        private readonly AsyncCallback forcePromptCallback;
        private readonly AsyncCallback reselectPromptCallback;

        // arguments for our two types of events
        private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();
        private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

        // the actual storage device
        private StorageDevice storageDevice;

        /// <summary>
        /// Gets whether or not the device is in a state to receive file operation method calls.
        /// </summary>
        public bool IsReady
        {
            get { return storageDevice != null && storageDevice.IsConnected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SaveDevice is enabled for use.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (EnabledChanged != null)
                        EnabledChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the order in which the SaveDevice is updated
        /// in the game. Components with a lower UpdateOrder are updated
        /// first.
        /// </summary>
        public int UpdateOrder
        {
            get { return updateOrder; }
            set
            {
                if (updateOrder != value)
                {
                    updateOrder = value;
                    if (UpdateOrderChanged != null)
                        UpdateOrderChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Invoked when a StorageDevice is selected.
        /// </summary>
        public event EventHandler<EventArgs> DeviceSelected;

        /// <summary>
        /// Invoked when a StorageDevice selector is canceled.
        /// </summary>
        public event EventHandler<SaveDeviceEventArgs> DeviceSelectorCanceled;

        /// <summary>
        /// Invoked when the user closes a prompt to reselect a StorageDevice.
        /// </summary>
        public event EventHandler<SaveDevicePromptEventArgs> DeviceReselectPromptClosed;

        /// <summary>
        /// Invoked when the StorageDevice is disconnected.
        /// </summary>
        public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

        /// <summary>
        /// Fired when the Enabled property has been changed.
        /// </summary>
        public event EventHandler<EventArgs> EnabledChanged;

        /// <summary>
        /// Fired when the UpdateOrder property has been changed.
        /// </summary>
        public event EventHandler<EventArgs> UpdateOrderChanged;

        static SaveDevice()
        {
            // reset the strings to fill in the defaults
            EasyStorageSettings.ResetSaveDeviceStrings();
        }

        /// <summary>
        /// Creates a new SaveDevice.
        /// </summary>
        protected SaveDevice()
        {
            storageDeviceSelectorCallback = StorageDeviceSelectorCallback;
            reselectPromptCallback = ReselectPromptCallback;
            forcePromptCallback = ForcePromptCallback;
        }

        /// <summary>
        /// Allows the SaveDevice to initialize itself.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Flags the SaveDevice to prompt for a storage device on the next Update.
        /// </summary>
        public void PromptForDevice()
        {
            // we only let the programmer show the selector if the 
            // SaveDevice isn't busy doing something else.
            if (state == SaveDevicePromptState.None)
                state = SaveDevicePromptState.ShowSelector;
        }

        /// <summary>
        /// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
        /// method with the desired parameters, using the given callback.
        /// </summary>
        /// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
        protected abstract void GetStorageDevice(AsyncCallback callback);

        /// <summary>
        /// Prepares the SaveDeviceEventArgs to be used for an event.
        /// </summary>
        /// <remarks>
        /// Subclasses can override this method to change the default argument values.
        /// </remarks>
        /// <param name="args">The event arguments to be configured.</param>
        protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
        {
            args.Response = SaveDeviceEventResponse.Prompt;

            // the PlayerSaveDevice overrides us to set this to the player
            // so we'll default to null under the assumption that it's ok
            // for any player to handle the message
            args.PlayerToPrompt = null;
        }

        /// <summary>
        /// Allows the component to update itself.
        /// </summary>
        /// <param name="gameTime">The current game timestamp.</param>
        public void Update(GameTime gameTime)
        {
#if XBOX
			// make sure gamer services are available for all of our Guide methods we use			
			if (!GamerServicesDispatcher.IsInitialized)
				throw new InvalidOperationException(Strings.NeedGamerService);
#endif

            bool deviceIsConnected = storageDevice != null && storageDevice.IsConnected;
            if (!deviceIsConnected && deviceWasConnected)
            {
                // if the device was disconnected, fire off the event and handle result
                PrepareEventArgs(eventArgs);

                if (DeviceDisconnected != null)
                    DeviceDisconnected(this, eventArgs);

                HandleEventArgResults();
            }
            else if (!deviceIsConnected)
            {
                // we use the try/catch because of the asynchronous nature of the Guide. 
                // the Guide may not be visible when we do our test, but it may open 
                // up after that point and before we've made a call, causing our Guide
                // methods to throw exceptions.
                try
                {
#if XBOX
					if (!Guide.IsVisible)
					{
#endif
                    switch (state)
                    {
                        // show the normal storage device selector
                        case SaveDevicePromptState.ShowSelector:
                            state = SaveDevicePromptState.None;
                            GetStorageDevice(storageDeviceSelectorCallback);
                            break;
                        // these actions don't apply anywhere but Xbox so we compile them out for Windows to prevent 
                        // issues with gamer services in redistributed games.
#if XBOX
							// the user cancelled the device selector, and we've decided to 
							// see if they want another chance to choose a device
							case SaveDevicePromptState.PromptForCanceled:
								ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForCancelledMessage, deviceOptionalOptions, reselectPromptCallback);
								break;

							// the user cancelled the device selector, and we've decided to
							// force them to choose again. this message is simply to inform
							// the user of that.	
							case SaveDevicePromptState.ForceCanceledReselection:
								ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceCancelledReselectionMessage, deviceRequiredOptions, forcePromptCallback);
								break;

							// the device has been disconnected, and we've decided to ask
							// the user if they want to choose a new one
							case SaveDevicePromptState.PromptForDisconnected:
								ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForDisconnectedMessage, deviceOptionalOptions, reselectPromptCallback);
								break;

							// the device has been disconnected, and we've decided to force
							// the user to select a new one. this message is simply to inform
							// the user of that.
							case SaveDevicePromptState.ForceDisconnectedReselection:
								ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceDisconnectedReselectionMessage, deviceRequiredOptions, forcePromptCallback);
								break;
#endif
                        default:
                            break;
                    }
#if XBOX
					}
#endif
                }

                // catch this one type of exception just to be safe
                catch (GuideAlreadyVisibleException) { }
            }

            deviceWasConnected = deviceIsConnected;
        }

        /// <summary>
        /// A callback for the BeginStorageDeviceSelectorPrompt.
        /// </summary>
        /// <param name="result">The result of the prompt.</param>
        private void StorageDeviceSelectorCallback(IAsyncResult result)
        {
            //get the storage device
            storageDevice = StorageDevice.EndShowSelector(result);

            // if we got a valid device, fire off the DeviceSelected event so
            // that the game knows we have a device
            if (storageDevice != null && storageDevice.IsConnected)
            {
                if (DeviceSelected != null)
                    DeviceSelected(this, null);
            }

            // if we don't have a valid device
            else
            {
                // prepare our event arguments for use
                PrepareEventArgs(eventArgs);

                // let the game know the device selector was cancelled so it
                // can tell us how to handle this
                if (DeviceSelectorCanceled != null)
                    DeviceSelectorCanceled(this, eventArgs);

                // handle the result of the event
                HandleEventArgResults();
            }
        }

        /// <summary>
        /// A callback for either of the message boxes telling users they
        /// have to choose a storage device, either from cancelling the
        /// device selector or disconnecting the device.
        /// </summary>
        /// <param name="result">The result of the prompt.</param>
        private void ForcePromptCallback(IAsyncResult result)
        {
            // just end the message and instruct the SaveDevice to show the selector
            Guide.EndShowMessageBox(result);
            state = SaveDevicePromptState.ShowSelector;
        }

        /// <summary>
        /// A callback for either of the message boxes asking the user
        /// to select a new device, either from cancelling the device
        /// seledctor or disconnecting the device.
        /// </summary>
        /// <param name="result">The result of the prompt.</param>
        private void ReselectPromptCallback(IAsyncResult result)
        {
            int? choice = Guide.EndShowMessageBox(result);

            // get the device if the user chose the first option
            state = choice.HasValue && choice.Value == 0 ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None;

            // fire an event for the game to know the result of the prompt
            promptEventArgs.ShowDeviceSelector = state == SaveDevicePromptState.ShowSelector;
            if (DeviceReselectPromptClosed != null)
                DeviceReselectPromptClosed(this, promptEventArgs);
        }

        /// <summary>
        /// Handles reading from the eventArgs to determine what action to take.
        /// </summary>
        private void HandleEventArgResults()
        {
            // clear the Device reference
            storageDevice = null;

            // determine the next action...
            switch (eventArgs.Response)
            {
                // will have the manager prompt the user with the option of reselecting the storage device
                case SaveDeviceEventResponse.Prompt:
                    state = deviceWasConnected
                        ? SaveDevicePromptState.PromptForDisconnected
                        : SaveDevicePromptState.PromptForCanceled;
                    break;

                // will have the manager prompt the user that the device must be selected
                case SaveDeviceEventResponse.Force:
                    state = deviceWasConnected
                        ? SaveDevicePromptState.ForceDisconnectedReselection
                        : SaveDevicePromptState.ForceCanceledReselection;
                    break;

                // will have the manager do nothing
                default:
                    state = SaveDevicePromptState.None;
                    break;
            }
        }

        // helper method for showing guide messages
        private static void ShowMessageBox(PlayerIndex? player, string title, string text, IEnumerable<string> buttons, AsyncCallback callback)
        {
            if (player.HasValue)
            {
                Guide.BeginShowMessageBox(player.Value, title, text, buttons, 0, MessageBoxIcon.None, callback, null);
            }
            else
            {
                Guide.BeginShowMessageBox(title, text, buttons, 0, MessageBoxIcon.None, callback, null);
            }
        }
    }
}