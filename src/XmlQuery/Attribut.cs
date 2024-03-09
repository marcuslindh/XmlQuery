namespace XmlQuery
{
    /// <summary>
    /// An attribut on an element
    /// </summary>
    public class Attribut
    {
        /// <summary>
        /// Name of the attribut
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Value of the attribut
        /// </summary>
        public string Value { get; set; } = "";

        public override string ToString()
        {
            return $"{Name} = '{Value}'";
        }
    }

}
