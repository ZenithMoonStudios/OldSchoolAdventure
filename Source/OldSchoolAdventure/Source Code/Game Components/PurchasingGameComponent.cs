using Destiny;
using Microsoft.Xna.Framework;

namespace OSA
{
    public class PurchasingGameComponent : GameComponent
    {
        enum States { None, PromptForPurchase, CheckForSignIn, SignIn, Purchase }
        States m_State;

        public delegate void PurchaseHandler(bool p_IsPurchased, object p_Tag);
        PurchaseHandler m_PurchaseHandler;
        object m_Tag;

        string m_Prompt;

        public PurchasingGameComponent(Game p_Game) : base(p_Game)
        {
        }

        public void EnsurePurchase(string p_Prompt, PurchaseHandler p_PurchaseHandler, object p_Tag)
        {
            m_Prompt = p_Prompt;
            m_PurchaseHandler = p_PurchaseHandler;
            m_Tag = p_Tag;
            if (GuideHelper.IsTrialMode)
            {
                m_State = States.PromptForPurchase;
            }
            else
            {
                m_State = States.None;
                m_PurchaseHandler(true, m_Tag);
            }
        }

        public void RequestPurchase(PurchaseHandler p_PurchaseHandler, object p_Tag)
        {
            m_Prompt = null;
            m_PurchaseHandler = p_PurchaseHandler;
            m_State = States.CheckForSignIn;
            m_Tag = p_Tag;
            if (!GuideHelper.IsTrialMode)
            {
                m_PurchaseHandler(true, m_Tag);
            }
        }

        public override void Update(GameTime p_GameTime)
        {
            // Do nothing if the guide is visible
            if (GuideHelper.IsVisible)
            {
                return;
            }

            if (m_State == States.None)
            {
                // Do nothing
            }
            else if (m_State == States.PromptForPurchase)
            {
                if (GuideHelper.IsTrialMode)
                {
                    MessageBoxScreen.Show(
                        this.ScreenManager,
                        "Purchase Game?",
                        m_Prompt,
                        "Yes", "No",
                        this.PromptForPurchaseMessageBoxHandler
                        );
                }
                else
                {
                    m_State = States.None;
                    m_PurchaseHandler(true, m_Tag);
                }
            }
            else if (m_State == States.CheckForSignIn)
            {
                if (GuideHelper.CanPurchase((PlayerIndex)this.InputState.ActivePlayerIndex))
                {
                    m_State = States.Purchase;
                }
                else
                {
                    MessageBoxScreen.Show(
                        this.ScreenManager,
                        "Sign In?",
                        "To purchase this game, you need to sign in with an Xbox LIVE account that can purchase content. Do you want to sign in now?",
                        "Yes", "No",
                        this.SignInMessageBoxHandler
                        );
                }
            }
            else if (m_State == States.SignIn)
            {
                // TODO: Not Implemented
                m_State = States.Purchase;
            }
            else if (m_State == States.Purchase)
            {
                GuideHelper.Purchase((PlayerIndex)this.InputState.ActivePlayerIndex);
                if (GuideHelper.IsTrialMode)
                {
                    if (string.IsNullOrEmpty(m_Prompt))
                    {
                        m_State = States.None;
                        m_PurchaseHandler(false, m_Tag);
                    }
                    else
                    {
                        m_State = States.PromptForPurchase;
                    }
                }
                else
                {
                    m_State = States.None;
                    m_PurchaseHandler(true, m_Tag);
                }
            }

            base.Update(p_GameTime);
        }

        void PromptForPurchaseMessageBoxHandler(bool p_IsAccept)
        {
            if (p_IsAccept)
            {
                m_State = States.CheckForSignIn;
            }
            else
            {
                m_State = States.None;
                m_PurchaseHandler(false, m_Tag);
            }
        }

        void SignInMessageBoxHandler(bool p_IsAccept)
        {
            if (p_IsAccept)
            {
                m_State = States.SignIn;
            }
            {
                m_State = States.None;
                m_PurchaseHandler(false, m_Tag);
            }
        }

        ScreenManager ScreenManager { get { return ((PlatformerGame)this.Game).ScreenManager; } }
        InputState InputState { get { return this.ScreenManager.InputState; } }
    }
}
