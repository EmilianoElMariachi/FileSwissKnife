namespace ElMariachi.FS.Tools.Splitting
{
    public enum NumPos
    {
        /// <summary>
        /// 123README.TXT
        /// </summary>
        BeforeBaseName,
        /// <summary>
        /// README123.TXT
        /// </summary>
        AfterBaseName,
        /// <summary>
        /// README.123TXT
        /// </summary>
        BeforeExt,
        /// <summary>
        /// README.TXT123
        /// </summary>
        AfterExt
    }
}