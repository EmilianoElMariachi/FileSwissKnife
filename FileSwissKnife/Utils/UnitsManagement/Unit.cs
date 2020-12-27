namespace FileSwissKnife.Utils.UnitsManagement
{
    public class Unit
    {
        public Unit(string name, int bytesFactor, SIUnit siUnit)
        {
            SIUnit = siUnit;
            BytesFactor = bytesFactor;
            Name = name;
        }

        public string Name { get; }

        public int BytesFactor { get; }

        public SIUnit SIUnit { get; }

        public long ToNbBytes(long nbUnits)
        {
            return BytesFactor * nbUnits;
        }

    }
}