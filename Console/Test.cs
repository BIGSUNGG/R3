using R3;

public abstract class TestR3
{
    public abstract void Test();
}

public class Test_1 : TestR3
{
    public override void Test()
    {
        // Console.ReadLine이 끝나면 토큰 취소
        CancellationTokenSource cts = new CancellationTokenSource();
        _ = Task.Run(() => { Console.ReadLine(); cts.Cancel(); });

        // 1초마다 Linq문 실행, Linq문에 넘어가는 x값은 n초이고 짝수 초마다 Subcribe된 함수 호출 및 TakeUntil의 토큰이 캔슬될 때까지 반복
        var subscription = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select((_, i) => i)
            .Where(x => x % 2 == 0)
            .TakeUntil(cts.Token)
            .Subscribe(x => Console.WriteLine($"Interval:{x}"));

        Observable.Timer(TimeSpan.FromSeconds(1))
            .TakeUntil(cts.Token);

        cts.Token.WaitHandle.WaitOne();
    }
}

public class Test_2 : TestR3
{
    string str = "ABC";

    public override void Test()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        // DefaultFrameProvider를 지정해야 사용할 수 있는 코드 (Unity, Blazor, Maui, Winform 등)
        //var subscription = Observable.EveryValueChanged(this, p => p.a)
        //    .Subscribe(s =>
        //        {
        //            Console.WriteLine(s);
        //            if (s == "AAA")
        //                cts.Cancel();
        //        });

        // Subscribe된 함수는 Observable 호출 시 기본적으로 한번 호출됨
        var subscription = Observable.Interval(TimeSpan.FromMilliseconds(10))
            .Select((_, i) => i)
            .DistinctUntilChanged()
            .TakeUntil(cts.Token)
            .Subscribe(u =>
            {
                Console.WriteLine($"str = {str}");
                if (str == "Finish")
                {
                    Console.WriteLine("Finish Test");
                    cts.Cancel();
                }

                str = Console.ReadLine();
            });

        cts.Token.WaitHandle.WaitOne();
    }
}