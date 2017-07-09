using System.Collections;


public class EnergyMonitoringSystem  {
    //MaterialDict all of it,#EMS
   private static EnergyMonitoringSystem instance;
   private EnergyMonitoringSystem() {}


   public static EnergyMonitoringSystem Instance
   {
      get 
      {
         if (instance == null)
         {
            instance = new EnergyMonitoringSystem();
         }
         return instance;
      }
   }


}

