namespace XmlQuery.Xml
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An attribut on an element.
    /// </summary>
    public class Attribut
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Attribut"/> class.
        /// </summary>
        /// <param name="name">name of the attribut.</param>
        /// <param name="value">value of the attribut.</param>
        public Attribut([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                this.Name = name;
            }

            if (string.IsNullOrEmpty(value) == false)
            {
                this.Value = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Attribut"/> class.
        /// </summary>
        public Attribut()
        {
        }

        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets Name of the attribut.
        /// </summary>
        [NotNull]
        public string Name
        {
            get
            {
                return this._name ?? string.Empty;
            }

            set
            {
                this._name = value ?? string.Empty;
            }
        }

        private string _value = string.Empty;

        /// <summary>
        /// Gets or Sets Value of the attribut.
        /// </summary>
        [NotNull]
        public string Value
        {
            get
            {
                return this._value ?? string.Empty;
            }

            set
            {
                this._value = value ?? string.Empty;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name} = '{this.Value}'";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Attribut attribut
                && string.Equals(this.Value, attribut._value, StringComparison.OrdinalIgnoreCase)
                && string.Equals(this.Name, attribut.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Value);
        }
    }
}
