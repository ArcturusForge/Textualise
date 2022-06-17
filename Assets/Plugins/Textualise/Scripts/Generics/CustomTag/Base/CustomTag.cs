namespace Arcturus.Textualise.Internal
{
    public abstract class CustomTag
    {
        /// <summary>
        /// Inline type casting to simplify usage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T : CustomTag
        {
            return (T)this;
        }
    }
}
