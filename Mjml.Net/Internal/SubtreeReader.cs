namespace Mjml.Net.Internal;

internal sealed class SubtreeReader : HtmlReaderWrapper
{
    private readonly int startDepth;
    private bool isEnded;

    public SubtreeReader(HtmlReaderWrapper parent)
        : base(parent)
    {
        startDepth = Depth;
    }

    public override bool Read()
    {
        // A nested reader could already have read the end tag of this subtree.
        if (isEnded || Depth < startDepth)
        {
            isEnded = true;
            return false;
        }

        // The subtree ends with the end tag of the current element or with the end of the input.
        if (!base.Read() || Depth < startDepth)
        {
            isEnded = true;
            return false;
        }

        return true;
    }
}
