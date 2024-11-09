namespace OSA
{
    /// <summary>
    /// Conversation transaction type helper class.
    /// </summary>
    public static class SpeakerConversationTransactionTypeHelper
    {
        public const string c_Add = "Add";
        public const string c_Remove = "Remove";

        public static SpeakerConversationTransactionType FromString(string p_TransactionType)
        {
            SpeakerConversationTransactionType result = SpeakerConversationTransactionType.Add;
            switch (p_TransactionType)
            {
                case c_Add: result = SpeakerConversationTransactionType.Add; break;
                case c_Remove: result = SpeakerConversationTransactionType.Remove; break;
            }
            return result;
        }

        public static string ToString(SpeakerConversationTransactionType p_TransactionType)
        {
            string result = c_Add;
            switch (p_TransactionType)
            {
                case SpeakerConversationTransactionType.Add: result = c_Add; break;
                case SpeakerConversationTransactionType.Remove: result = c_Remove; break;
            }
            return result;
        }
    }
}
