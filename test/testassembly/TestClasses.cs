namespace TestAssembly
{
    public interface ITA_T1 { }

    public interface ITA_T2 { }

    public interface ITA_TG<T> { }

    public abstract class TA_A { }

    public interface ITA_TGM<T1, T2> { }

    public class TA_T1 : ITA_T1 { }

    public class TA_TM1 : ITA_T1 { }

    public class TA_TM2 : ITA_T1 { }

    public class TA_TM3 : ITA_T1 { }

    public class TA_T2 : ITA_T1, ITA_T2 { }

    public class TA_T3 : ITA_T1, ITA_T2 { }

    public class TA_TG<T> : ITA_TG<T> { }

    public class TA_TGC1 : ITA_TG<int> { }

    public class TA_TGC2 : ITA_TG<string> { }

    public class TA_TGC3 : ITA_TG<double> { }
    
    public class TA_TGM<T1, T2> : ITA_TGM<T1, T2> { }

    public class TA_TGMI<T1> : ITA_TGM<T1, int> { }

    public class TA_AI1 : TA_A { }

    public class TA_AI2 : TA_A, ITA_T1 { }
}
