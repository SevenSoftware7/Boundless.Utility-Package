namespace Seven.Boundless.Utility;

using Godot;

/// <summary>
/// Timer is used to measure how much time has passed since it was started
/// </summary>
public class Timer {
	/// <summary>
	/// The game clock time, in Milliseconds, at which the timer started, see <seealso cref="Time.GetTicksMsec"/>
	/// </summary>
	public ulong startTime = 0;
	/// <summary>
	/// The total time duration, in Milliseconds, which has passed since the last time the timer started, see <seealso cref="Start"/>
	/// </summary>
	public ulong Duration => Time.GetTicksMsec() - startTime;


	/// <summary>
	/// Creates a timer, you can choose to start the timer on construction with the <paramref name="start"/> parameter
	/// </summary>
	/// <param name="start">Whether the timer should start immediately on construction or not</param>
	public Timer(bool start = true) {
		if (start) Start();
	}

	/// <summary>
	/// Start measuring how much time is passing (in Milliseconds)
	/// </summary>
	public void Start() {
		startTime = Time.GetTicksMsec();
	}

	/// <summary>
	/// Returns the duration in Milliseconds that has oassed since the timer last started.
	/// </summary>
	/// <param name="timer">The timer duration in Milliseconds, see <seealso cref="Duration"/></param>
	public static implicit operator ulong(Timer timer) => timer.Duration;
}