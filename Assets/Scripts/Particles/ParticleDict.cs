using UnityEngine;
using System.Collections;
using System;

public class ParticleDict
{

    #region Singleton
    private static ParticleDict instance;

   private ParticleDict() { }

   public static ParticleDict Instance
   {
      get 
      {
         if (instance == null)
         {
             instance = new ParticleDict();
         }
         return instance;
      }
   }
#endregion


    //public void Fire

   public class ParticleLaunchParameters
   {
       public Vector3 particlePos;
       public Vector3 velocity;
       public float size;
       public float lifespan;
       public Color color;

       public ParticleLaunchParameters(Vector3 _particlePos, Vector3 _velocity, float _size, float _lifespan, Color _color)
       {
           particlePos = _particlePos;
           velocity = _velocity;
           size = _size;
           lifespan = _lifespan;
           color = _color;
       }
   }
}
