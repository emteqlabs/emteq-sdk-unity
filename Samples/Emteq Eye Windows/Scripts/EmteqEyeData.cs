#if EMTEQ_EYE
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR;
using ViveSR.anipal.Eye;
using Unity.Mathematics;


namespace EmteqLabs
{

#region EyeShape_v2
    [Serializable]
    public enum EyeShape_v2
    {
        None = -1,
        Eye_Left_Blink = 0,
        Eye_Left_Wide,
        Eye_Left_Right,
        Eye_Left_Left,
        Eye_Left_Up,
        Eye_Left_Down,
        Eye_Right_Blink = 6,
        Eye_Right_Wide,
        Eye_Right_Right,
        Eye_Right_Left,
        Eye_Right_Up,
        Eye_Right_Down,
        Eye_Frown = 12,
        Eye_Left_Squeeze,
        Eye_Right_Squeeze,
        Max = 15,
    }

    [Serializable]
    public class EyeShapeTable_v2
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public EyeShape_v2[] eyeShapes;
    }
#endregion

    [Serializable]
    public class SingleEyeExpression
    {
        public SingleEyeExpression(ViveSR.anipal.Eye.SingleEyeExpression singleEyeExpression)
        {
            eye_wide = singleEyeExpression.eye_wide;
            eye_squeeze = singleEyeExpression.eye_squeeze;
            eye_frown = singleEyeExpression.eye_frown;
        }

        public float eye_wide; /*!<A value representing how open eye widely.*/
        public float eye_squeeze; /*!<A value representing how the eye is closed tightly.*/
        public float eye_frown; /*!<A value representing user's frown.*/
    };

    [Serializable]
    public class EyeExpression
    {
        public EyeExpression(ViveSR.anipal.Eye.EyeExpression eyeExpression)
        {
            left = new SingleEyeExpression(eyeExpression.left);
            right = new SingleEyeExpression(eyeExpression.right);
        }

        public SingleEyeExpression left;
        public SingleEyeExpression right;
    };

    [Serializable]
    public class EmteqEyeData
    {
        public EmteqEyeData(EyeData_v2 eyeDataV2)
        {
            no_user = eyeDataV2.no_user;
            frame_sequence = eyeDataV2.frame_sequence;
            timestamp = eyeDataV2.timestamp;
            verbose_data = new VerboseData(eyeDataV2.verbose_data);
            expression_data = new EyeExpression(eyeDataV2.expression_data);
            timestampUnix = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
            timestampJ2000 = timestampUnix - 946684800000U; //UnixEpochOffset
        }

        /** Indicate if there is a user in front of HMD. */
        public bool no_user;
        /** The frame sequence.*/
        public int frame_sequence;
        /** The time when the frame was capturing. in millisecond.*/
        public int timestamp;
        public ulong timestampUnix;
        public ulong timestampJ2000;
        public VerboseData verbose_data;
        public EyeExpression expression_data;
        
    }

    [Serializable]
    public class SingleEyeData
    {
        public SingleEyeData(ViveSR.anipal.Eye.SingleEyeData singleEyeData)
        {
            eye_data_validata_bit_mask = singleEyeData.eye_data_validata_bit_mask;
            gaze_origin_mm = singleEyeData.gaze_origin_mm;
            gaze_direction_normalized = singleEyeData.gaze_direction_normalized;
            pupil_diameter_mm = singleEyeData.pupil_diameter_mm;
            eye_openness = singleEyeData.eye_openness;
            pupil_position_in_sensor_area = singleEyeData.pupil_position_in_sensor_area;
        }

        /** The bits containing all validity for this frame.*/
        public System.UInt64 eye_data_validata_bit_mask;
        /** The point in the eye from which the gaze ray originates in millimeter.(right-handed coordinate system)*/
        public float3 gaze_origin_mm;
        /** The normalized gaze direction of the eye in [0,1].(right-handed coordinate system)*/
        public float3 gaze_direction_normalized;
        /** The diameter of the pupil in millimeter*/
        public float pupil_diameter_mm;
        /** A value representing how open the eye is.*/
        public float eye_openness;
        /** The normalized position of a pupil in [0,1]*/
        public float2 pupil_position_in_sensor_area;

        public bool GetValidity(SingleEyeDataValidity validity)
        {
            return (eye_data_validata_bit_mask & (ulong)(1 << (int)validity)) > 0;
        }
    }

    [Serializable]
    public class CombinedEyeData
    {
        public CombinedEyeData(ViveSR.anipal.Eye.CombinedEyeData combinedEyeData)
        {
            eye_data = new SingleEyeData(combinedEyeData.eye_data);
            convergence_distance_validity = combinedEyeData.convergence_distance_validity;
            convergence_distance_mm = combinedEyeData.convergence_distance_mm;
        }

        public SingleEyeData eye_data;
        public bool convergence_distance_validity;
        public float convergence_distance_mm;
    }

    [Serializable]
    public class VerboseData
    {
        public VerboseData(ViveSR.anipal.Eye.VerboseData verboseData)
        {
            left = new SingleEyeData(verboseData.left);
            right = new SingleEyeData(verboseData.right);
            combined = new CombinedEyeData(verboseData.combined);
            tracking_improvements = new TrackingImprovements(verboseData.tracking_improvements);
        }

        /** A instance of the struct as @ref EyeData related to the left eye*/
        public SingleEyeData left;
        /** A instance of the struct as @ref EyeData related to the right eye*/
        public SingleEyeData right;
        /** A instance of the struct as @ref EyeData related to the combined eye*/
        public CombinedEyeData combined;
        public TrackingImprovements tracking_improvements;
    }

    [Serializable]
    public enum TrackingImprovement : int
    {
        TRACKING_IMPROVEMENT_USER_POSITION_HMD,
        TRACKING_IMPROVEMENT_CALIBRATION_CONTAINS_POOR_DATA,
        TRACKING_IMPROVEMENT_CALIBRATION_DIFFERENT_BRIGHTNESS,
        TRACKING_IMPROVEMENT_IMAGE_QUALITY,
        TRACKING_IMPROVEMENT_INCREASE_EYE_RELIEF,
    };

    [Serializable]
    public class TrackingImprovements
    {
        public TrackingImprovements(ViveSR.anipal.Eye.TrackingImprovements trackingImprovements)
        {
            count = trackingImprovements.count;
            items = trackingImprovements.items.Select(x => (TrackingImprovement)(int)x).ToArray();
        }

        public int count;
        public TrackingImprovement[] items;
    };
}
#endif