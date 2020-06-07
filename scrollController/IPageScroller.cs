namespace ScrollShared
{
	public interface IPageScroller
	{
		void Scroll(ScrollingDirection direction, double value);
	}

	public interface IParameters
	{
		ScrollingSpeeds SpeedLever { get; }
		ScrollingFPS FPS { get; }
	}

	public enum ScrollingSpeeds
	{
		Slow = 1,
		Normal = 2,
		Fast = 3,
		Very_Fast = 4,
	}

	public enum ScrollingDirection
	{
		None,
		Vertical = 1,
		Horizontal = 2
	}
	public enum ScrollingFPS
	{
		Very_Low = 1,
		Low = 2,
		Normal = 3,
		High = 4,
		Very_High = 5,
	}
}
