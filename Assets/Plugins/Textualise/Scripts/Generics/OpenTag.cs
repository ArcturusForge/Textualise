namespace Arcturus.Textualise.Internal
{
    public struct OpenTag
    {
        public string Opening { get; private set; }
        public string Closing { get; private set; }
        public string Data { get; private set; }

        public OpenTag(string opening, string closing, string data = "")
        {
            Opening = opening;
            Closing = closing;
            Data = data;
        }
    }
}
