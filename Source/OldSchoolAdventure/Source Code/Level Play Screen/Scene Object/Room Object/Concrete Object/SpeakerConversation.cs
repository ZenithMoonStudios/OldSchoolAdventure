using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Conversation class.
    /// </summary>
    public class SpeakerConversation
    {
        GameConditionList m_Conditions = new GameConditionList();
        SpeakerConversationSentenceList m_Sentences = new SpeakerConversationSentenceList();
        SpeakerConversationTransactionList m_Transactions = new SpeakerConversationTransactionList();

        public GameConditionList Conditions { get { return m_Conditions; } }
        public SpeakerConversationSentenceList Sentences { get { return m_Sentences; } }
        public SpeakerConversationTransactionList Transactions { get { return m_Transactions; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeakerConversation(
            GameConditionList p_Conditions,
            SpeakerConversationSentenceList p_Sentences,
            SpeakerConversationTransactionList p_Transactions
            )
        {
            int i;
            // Populate conditions
            if (p_Conditions != null)
            {
                for (i = 0; i < p_Conditions.Count; i++)
                {
                    m_Conditions.Add(p_Conditions[i]);
                }
            }
            // Populate sentences
            for (i = 0; i < p_Sentences.Count; i++)
            {
                m_Sentences.Add(p_Sentences[i]);
            }
            // Populate transactions
            if (p_Transactions != null)
            {
                for (i = 0; i < p_Transactions.Count; i++)
                {
                    m_Transactions.Add(p_Transactions[i]);
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeakerConversation(SpeakerConversationSentence p_Sentence)
        {
            m_Sentences.Add(p_Sentence);
        }

        /// <summary>
        /// All conditions in this conversation are true.
        /// </summary>
        public bool CanSpeak(CollectibleStore p_Store)
        {
            return m_Conditions.IsTrue(p_Store);
        }

        /// <summary>
        /// Execute transactions.
        /// </summary>
        public void ExecuteTransactions(CollectibleStore p_Store)
        {
            for (int i = 0; i < m_Transactions.Count; i++)
            {
                m_Transactions[i].Execute(p_Store);
            }
        }
    }

    /// <summary>
    /// Conversation list class.
    /// </summary>
    public class SpeakerConversationList : List<SpeakerConversation> { }
}
