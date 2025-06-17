using UnityEngine;

public partial class Orchard
{
    public enum TimeSource
    {
        FixedDelta,
        ScaledFixedDelta,
        RealDelta,
        NetworkTime
    }
    private TimeSource _currentTimeSource = TimeSource.RealDelta;
	
    public TimeSource CurrentTimeSource
    {
        get => _currentTimeSource;
        set => _currentTimeSource = value; 
    }

    public float GameTime
    {
        get
        {
            switch (_currentTimeSource)
            {
                case TimeSource.FixedDelta:
                    return FixedDeltaTime;

                case TimeSource.ScaledFixedDelta:
                    return ScaledFixedDeltaTime;

                case TimeSource.RealDelta:
                    return DeltaTime;

                case TimeSource.NetworkTime:
                    return NetworkTime;

                default:
             
                    return Time.deltaTime;
            }
        }
    }
}
