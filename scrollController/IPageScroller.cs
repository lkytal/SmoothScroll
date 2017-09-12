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
		Vertical = 1,
		Horizental = 2
	}
}
