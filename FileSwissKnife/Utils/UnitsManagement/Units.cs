using System;
using System.Collections;
using System.Collections.Generic;
using FileSwissKnife.Localization;

namespace FileSwissKnife.Utils.UnitsManagement
{
    /// <summary>
    /// Represents the list of available units
    /// </summary>
    public class Units : IEnumerable<Unit>
    {
        private readonly List<Unit> _units = new List<Unit>();

        private const int DEC_KILO = 1000;
        private const int BIN_KILO = 1024;

        private Units()
        {
            _units.Add(Byte);
            _units.Add(KB);
            _units.Add(KiB);
            _units.Add(MB);
            _units.Add(MiB);
            _units.Add(GB);
            _units.Add(GiB);
        }

        public static Units All { get; } = new Units();

        public Unit Byte { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitB, (int)Math.Pow(DEC_KILO, 0), SIUnit.B);

        public Unit KB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitKB, (int)Math.Pow(DEC_KILO, 1), SIUnit.KB);

        public Unit KiB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitKiB, (int)Math.Pow(BIN_KILO, 1), SIUnit.KiB);

        public Unit MB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitMB, (int)Math.Pow(DEC_KILO, 2), SIUnit.MB);

        public Unit MiB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitMiB, (int)Math.Pow(BIN_KILO, 2), SIUnit.MiB);

        public Unit GB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitGB, (int)Math.Pow(DEC_KILO, 3), SIUnit.GB);

        public Unit GiB { get; } = new Unit(LocalizationManager.Instance.Current.Keys.UnitGiB, (int)Math.Pow(BIN_KILO, 3), SIUnit.GiB);

        public IEnumerator<Unit> GetEnumerator()
        {
            return _units.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}