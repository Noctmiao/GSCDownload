using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidanceStsbilityCommsDownLoad
{
    [Serializable]
    public enum EnumUnit
    {   // totalNumberOfUnits = 81
        UNKNOWN = 0,
        METERS, FEET, INCHES, MILLIMETERS, CENTIMETERS,
        OHMM,
        S_P_M, MMHO_P_M, US_P_CM,
        GR_API,
        COUNT_CPS,
        KG_P_M3, G_P_CM3, KG_L, LB_P_GAL, LB_P_FT3, PSI_F, PSI_KF, KPA_M, BAR_M,
        MIC_P_FT, MIC_P_M,
        DEGREES, RADIANS,
        DIMENSIONLESS,
        RATIO, PERCENT, POROSITY_UNITS,
        M_P_HR, FT_HR, MIN_P_FT, MIN_P_M, MIN_P_5FT, FT_P_MIN, M_P_MIN,
        MGM, KG, KLBF, LBF, KDN, TONNE,
        KPA, PSI, KG_P_CM2, BARS, PA, KPSI,
        ST_P_M,
        R_P_M,
        DECIBELS,
        CELSIUS, FAHRENHEIT, KELVIN, RANKINE,
        CM3, LITRES, M3, IN3, FT3, BARRELS, GALUS, GALIMP,
        K_WT, KCL_WT, KOH_WT, KCL_L_B, KOH_L_B, KCL_M_L, KOH_M_L, KCL_KG_M3, KOH_KG_M3,
        SECONDS, MINUTES, HOURS, DAYS,
        DEG_P_M, RAD_P_M, DEG_P_FT, DEG_P_30M, DEG_P_100FT = 80,
    };
}
