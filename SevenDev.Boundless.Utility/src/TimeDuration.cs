namespace SevenDev.Boundless.Utility;

using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// TimeDuration is used to check whether a specific time duration has passed or not.
/// </summary>
public class TimeDuration {
	/// <summary>
	/// The duration, in Milliseconds which will be awaited before the TimeDuration
	/// </summary>
	public ulong DurationMsec;

	/// <summary>
	/// The game clock time, in Milliseconds, at which the TimeDuration will have elapsed, see <see cref="Time.GetTicksMsec"/>.
	/// </summary>
	public ulong StopTime { get; private set; }

	/// <summary>
	/// An event which will be invoked whenever the Measured TimeDuration is elapsed.<para/>
	/// The event contains a reference to the TimeDuration which elapsed and a boolean value representing whether the event was invoked upon timing cancellation (false for cancelled, true for completed), for ease of use.
	/// </summary>
	public event Action<TimeDuration, bool>? OnElapsed;
	private readonly CancellationTokenSource _onElapsedCancellationToken = new();

	/// <summary>
	/// Whether the given TimeDuration has elapsed or is stopped
	/// </summary>
	public bool HasElapsed => Time.GetTicksMsec() >= StopTime;

	/// <summary>
	/// The amount of time, in Milliseconds, which is left to wait before the TimeDuration is elapsed.<para/>
	/// This will always return 0 if the TimeDuration has elapsed, see <see cref="HasElapsed"/>.
	/// </summary>
	public ulong TimeLeft => HasElapsed ? 0 : StopTime - Time.GetTicksMsec();
	/// <summary>
	/// The amount of time, in Milliseconds, which has elapsed since the TimeDuration elapsed.<para/>
	/// This will always return 0 if the TimeDuration has not elapsed, see <see cref="HasElapsed"/>.
	/// </summary>
	public ulong Overtime => HasElapsed ? Time.GetTicksMsec() - StopTime : 0;

	/// <summary>
	/// Constructs a TimeDuration with a given duration in Milliseconds.<para/>
	/// Can be used to start measuring time immediately after construction.
	/// </summary>
	/// <param name="durationMsec">The duration which will have to pass for the TimeDuration to be considered Elapsed</param>
	/// <param name="start">Whether the TimeDuration should start measuring time immediately after construction</param>
	public TimeDuration(ulong durationMsec, bool start = false) {
		DurationMsec = durationMsec;
		if (start) Start();
	}

	/// <summary>
	/// Stop the timing process. Immediately invokes <see cref="OnElapsed"/> with a false completion flag.
	/// </summary>
	public void End() {
		StopTime = Time.GetTicksMsec();

		_onElapsedCancellationToken.Cancel();
		OnElapsed?.Invoke(this, false);
	}

	/// <summary>
	/// Start the timing process. Will invoke <see cref="OnElapsed"/> after <see cref="DurationMsec"/> Milliseconds.
	/// </summary>
	public void Start() {
		StopTime = Time.GetTicksMsec() + DurationMsec;

		Task.Run(async () => {
			await Task.Delay((int)DurationMsec);
			OnElapsed?.Invoke(this, true);
		}, _onElapsedCancellationToken.Token);
	}

	/// <summary>
	/// Returns the amount of time, in Milliseconds, left before the TimeDuration will have elapsed.
	/// See <see cref="TimeLeft"/>.
	/// </summary>
	/// <param name="timeDuration">The TimeDuration whose remaining time will be returned</param>
	public static implicit operator float(TimeDuration timeDuration) => timeDuration.TimeLeft;

	/// <summary>
	/// Returns whether <see cref="DurationMsec"/> Milliseconds have elapsed since the timing process has last started (see <see cref="Start"/>).<para/>
	/// See <see cref="HasElapsed"/>.
	/// </summary>
	/// <param name="timeDuration">The TimeDuration whose Elapsed state will be returned</param>
	public static implicit operator bool(TimeDuration timeDuration) => timeDuration.HasElapsed;
}