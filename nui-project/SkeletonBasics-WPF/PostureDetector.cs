using System;

public class PostureDetector
{
    const float Epsilon = 0.1f;
    const float MaxRange = 0.25f;
    const int AccumulatorTarget = 10;

    Posture previousPosture = Posture.None;
    public event Action<Posture> PostureDetected;
    int accumulator;
    Posture accumulatedPosture = Posture.None;

    public Posture CurrentPosture
    {
        get { return previousPosture; }
    }

    public void TrackPostures(ReplaySkeletonData skeleton)
    {
        if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
            return;

        Vector3? headPosition = null;
        Vector3? leftHandPosition = null;
        Vector3? rightHandPosition = null;

        foreach (Joint joint in skeleton.Joints)
        {
            if (joint.Position.W < 0.8f || joint.TrackingState != JointTrackingState.Tracked)
                continue;

            switch (joint.ID)
            {
                case JointID.Head:
                    headPosition = joint.Position.ToVector3();
                    break;
                case JointID.HandLeft:
                    leftHandPosition = joint.Position.ToVector3();
                    break;
                case JointID.HandRight:
                    rightHandPosition = joint.Position.ToVector3();
                    break;
            }
        }

        // HandsJoined
        if (CheckHandsJoined(rightHandPosition, leftHandPosition))
            return;

        // LeftHandOverHead
        if (CheckHandOverHead(headPosition, leftHandPosition))
        {
            RaisePostureDetected(Posture.LeftHandOverHead);
            return;
        }

        // RightHandOverHead
        if (CheckHandOverHead(headPosition, rightHandPosition))
        {
            RaisePostureDetected(Posture.RightHandOverHead);
            return;
        }

        // LeftHello
        if (CheckHello(headPosition, leftHandPosition))
        {
            RaisePostureDetected(Posture.LeftHello);
            return;
        }

        // RightHello
        if (CheckHello(headPosition, rightHandPosition))
        {
            RaisePostureDetected(Posture.RightHello);
            return;
        }

        previousPosture = Posture.None;
        accumulator = 0;
    }

    bool CheckHandOverHead(Vector3? headPosition, Vector3? handPosition)
    {
        if (!handPosition.HasValue || !headPosition.HasValue)
            return false;

        if (handPosition.Value.Y < headPosition.Value.Y)
            return false;

        if (Math.Abs(handPosition.Value.X - headPosition.Value.X) > MaxRange)
            return false;

        if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) > MaxRange)
            return false;

        return true;
    }


    bool CheckHello(Vector3? headPosition, Vector3? handPosition)
    {
        if (!handPosition.HasValue || !headPosition.HasValue)
            return false;

        if (Math.Abs(handPosition.Value.X - headPosition.Value.X) < MaxRange)
            return false;

        if (Math.Abs(handPosition.Value.Y - headPosition.Value.Y) > MaxRange)
            return false;

        if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) > MaxRange)
            return false;

        return true;
    }

    bool CheckHandsJoined(Vector3? leftHandPosition, Vector3? rightHandPosition)
    {
        if (!leftHandPosition.HasValue || !rightHandPosition.HasValue)
            return false;

        float distance = (leftHandPosition.Value - rightHandPosition.Value).Length();

        if (distance > Epsilon)
            return false;

        RaisePostureDetected(Posture.HandsJoined);
        return true;
    }

    void RaisePostureDetected(Posture posture)
    {
        if (accumulator < AccumulatorTarget)
        {
            if (accumulatedPosture != posture)
            {
                accumulator = 0;
                accumulatedPosture = posture;
            }
            accumulator++;
            return;
        }

        if (previousPosture == posture)
            return;

        previousPosture = posture;
        if (PostureDetected != null)
            PostureDetected(posture);

        accumulator = 0;
    }
}