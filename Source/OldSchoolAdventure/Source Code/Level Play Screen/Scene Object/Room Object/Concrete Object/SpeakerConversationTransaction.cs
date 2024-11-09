using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Conversation transaction class.
    /// </summary>
    public class SpeakerConversationTransaction
    {
        public SpeakerConversationTransactionType Type { get; private set; }
        public string ObjectTypePath { get; private set; }
        public int Quantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeakerConversationTransaction(SpeakerConversationTransactionType p_Type, string p_ObjectTypePath, int p_Quantity)
        {
            this.Type = p_Type;
            this.ObjectTypePath = p_ObjectTypePath;
            this.Quantity = p_Quantity;
        }

        /// <summary>
        /// Execute the transaction.
        /// </summary>
        public void Execute(CollectibleStore p_Store)
        {
            if (this.Type == SpeakerConversationTransactionType.Add)
            {
                p_Store.Collect(this.ObjectTypePath, this.Quantity);
            }
            else if (this.Type == SpeakerConversationTransactionType.Remove)
            {
                p_Store.Use(this.ObjectTypePath, this.Quantity);
            }
        }
    }

    /// <summary>
    /// Conversation transaction list.
    /// </summary>
    public class SpeakerConversationTransactionList : List<SpeakerConversationTransaction> { }

    /// <summary>
    /// Xml conversation transaction list p_Reader.
    /// </summary>
    public static class XmlSpeakerConversationTransactionListReader
    {
        /// <summary>
        /// Create an instance.
        /// </summary>
        public static SpeakerConversationTransactionList Load(List<OSATypes.Transactions> p_transactions)
        {
            SpeakerConversationTransactionList transactions = new SpeakerConversationTransactionList();
            if (p_transactions != null && p_transactions.Count > 0)
            {
                foreach (OSATypes.Transactions p_XmlTransactionsNode in p_transactions)
                    if (p_XmlTransactionsNode.Transaction != null && p_XmlTransactionsNode.Transaction.Count > 0)
                        for (int j = 0; j < p_XmlTransactionsNode.Transaction.Count; j++)
                        {
                            OSATypes.TransactionsTransaction xmlTransactionNode = p_XmlTransactionsNode.Transaction[j];
                            SpeakerConversationTransactionType transactionType = SpeakerConversationTransactionTypeHelper.FromString(xmlTransactionNode.Type != null ? xmlTransactionNode.Type : "Add");
                            string item = xmlTransactionNode.Item != null ? xmlTransactionNode.Item : string.Empty;
                            int quantity = xmlTransactionNode.Quantity != null && int.TryParse(xmlTransactionNode.Quantity, out quantity) ? int.Parse(xmlTransactionNode.Quantity) : 1;
                            transactions.Add(new SpeakerConversationTransaction(transactionType, item, quantity));
                        }
            }
            return transactions;
        }
    }

}
