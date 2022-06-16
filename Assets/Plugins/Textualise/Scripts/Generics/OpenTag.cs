namespace Arcturus.Textualise.Internal
{
    public struct OpenTag
    {
        public string Opening { get; private set; }
        public string Closing { get; private set; }

        public OpenTag(string opening, string closing)
        {
            Opening = opening;
            Closing = closing;
        }
    }
}
