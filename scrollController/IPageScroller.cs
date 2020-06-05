namespace ScrollShared
{
	public interface IPageScroller
	{
		void Scroll(ScrollingDirection direction, double value);
	}

	public enum ScrollingSpeeds
	{
		Slow = 1,
		Normal = 2,
		Fast = 3
	}

	public enum ScrollingDirection
	{
		None,
		Vertical = 1,
		Horizontal = 2
	}
	public enum ScrollingFPS
	{
		None,
		Low = 1,
		Normal = 2,
		High = 3,
		Very_High = 4,
	}
}
