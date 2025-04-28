namespace SevenDev.Boundless.Utility;

using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Countdown is used to check whether a specific time duration has passed or not.
/// </summary>
public class Countdown {
	/// <summary>
	/// The duration, in Milliseconds which will be awaited before the Countdown
	/// </summary>
	public ulong DurationMsec;

	/// <summary>
	/// The game clock time, in Milliseconds, at which the Countdown will be completed, see <see cref="Time.GetTicksMsec"/>.
	/// </summary>
	public ulong StopTime { get; private set; }

	/// <summary>
	/// An event which will be invoked whenever the Countdown is complete.<para/>
	/// The event contains a reference to the Countdown which completes and a boolean value representing whether the event was invoked upon timing cancellation (false for cancelled, true for completed), for ease of use.
	/// </summary>
	public event Action<Countdown, bool>? OnCompleted;
	private readonly CancellationTokenSource _onCompletedCancellationToken = new();

	/// <summary>
	/// Whether the given Countdown is completed or is stopped
	/// </summary>
	public bool IsCompleted => Time.GetTicksMsec() >= StopTime;

	/// <summary>
	/// The amount of time, in Milliseconds, which is left to wait before the Countdown is completed.<para/>
	/// This will always return 0 if the Countdown was completed, see <see cref="IsCompleted"/>.
	/// </summary>
	public ulong TimeLeft => IsCompleted ? 0 : StopTime - Time.GetTicksMsec();
	/// <summary>
	/// The amount of time, in Milliseconds, which has elapsed since the Countdown completed.<para/>
	/// This will always return 0 if the Countdown was not completed, see <see cref="IsCompleted"/>.
	/// </summary>
	public ulong Overtime => IsCompleted ? Time.GetTicksMsec() - StopTime : 0;

	/// <summary>
	/// Constructs a Countdown with a given duration in Milliseconds.<para/>
	/// Can be used to start measuring time immediately after construction.
	/// </summary>
	/// <param name="durationMsec">The duration which will have to pass for the Countdown to be considered completed</param>
	/// <param name="start">Whether the Countdown should start measuring time immediately after construction</param>
	public Countdown(ulong durationMsec, bool start = false) {
		DurationMsec = durationMsec;
		if (start) Start();
	}

	/// <summary>
	/// Stop the countdown process. Immediately invokes <see cref="OnCompleted"/> with a false completion flag.
	/// </summary>
	public void End() {
		StopTime = Time.GetTicksMsec();

		_onCompletedCancellationToken.Cancel();
		OnCompleted?.Invoke(this, false);
	}

	/// <summary>
	/// Start the countdown process. Will invoke <see cref="OnCompleted"/> after <see cref="DurationMsec"/> Milliseconds.
	/// </summary>
	public void Start() {
		StopTime = Time.GetTicksMsec() + DurationMsec;

		Task.Run(async () => {
			await Task.Delay((int)DurationMsec);
			OnCompleted?.Invoke(this, true);
		}, _onCompletedCancellationToken.Token);
	}

	/// <summary>
	/// Returns the amount of time, in Milliseconds, left before the Countdown will be completed.
	/// See <see cref="TimeLeft"/>.
	/// </summary>
	/// <param name="countdown">The Countdown whose remaining time will be returned</param>
	public static implicit operator float(Countdown countdown) => countdown.TimeLeft;

	/// <summary>
	/// Returns whether <see cref="DurationMsec"/> Milliseconds have elapsed since the timing process has last started (see <see cref="Start"/>).<para/>
	/// See <see cref="IsCompleted"/>.
	/// </summary>
	/// <param name="countdown">The Countdown whose completion state will be returned</param>
	public static implicit operator bool(Countdown countdown) => countdown.IsCompleted;
}