namespace Playground.Manager.Inventory.Meta.Attributes
{
    public class AttributeModifier
    {
        private Attribute _attribute;
        private double _value;

        public AttributeModifier(Attribute attribute, double value)
        {
            _attribute = attribute;
            _value = value;
        }

        public Attribute Attribute => _attribute;

        public double Value
        {
            get => _value;
            set => _value = value;
        }
    }
}