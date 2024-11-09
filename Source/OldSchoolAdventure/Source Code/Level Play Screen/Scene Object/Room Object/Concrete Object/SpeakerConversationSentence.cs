using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Conversation sentence class.
    /// </summary>
    public class SpeakerConversationSentence
    {
        public string SpeakerObjectTypePath { get; private set; }
        public string Content { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeakerConversationSentence(string p_SpeakerObjectTypePath, string p_Content)
        {
            this.SpeakerObjectTypePath = p_SpeakerObjectTypePath;
            this.Content = p_Content;
        }
    }

    /// <summary>
    /// Conversation sentence list.
    /// </summary>
    public class SpeakerConversationSentenceList : List<SpeakerConversationSentence> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public static class XmlSpeakerConversationSentenceListReader
    {
        /// <summary>
        /// Create an instance.
        /// </summary>
        public static SpeakerConversationSentenceList Load(List<OSATypes.Sentences> p_sentences)
        {
            SpeakerConversationSentenceList sentences = new SpeakerConversationSentenceList();
            if (p_sentences != null && p_sentences.Count > 0)
            {
                foreach (OSATypes.Sentences p_XmlSentencesNode in p_sentences)
                    if (p_XmlSentencesNode.Sentence != null && p_XmlSentencesNode.Sentence.Count > 0)
                        for (int j = 0; j < p_XmlSentencesNode.Sentence.Count; j++)
                        {
                            OSATypes.SentencesSentence xmlSentenceNode = p_XmlSentencesNode.Sentence[j];
                            string speaker = xmlSentenceNode.Speaker != null ? xmlSentenceNode.Speaker : string.Empty;
                            string content = xmlSentenceNode.Value != null ? xmlSentenceNode.Value : string.Empty;
                            sentences.Add(new SpeakerConversationSentence(speaker, content));
                        }
            }
            return sentences;
        }
    }

}
