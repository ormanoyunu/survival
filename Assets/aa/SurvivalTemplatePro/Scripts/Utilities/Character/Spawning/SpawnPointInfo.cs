using UnityEngine;

namespace SurvivalTemplatePro.Gameplay
{
    public struct SpawnPointInfo
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public static SpawnPointInfo Default => defaultSpawnPoint;

        private static readonly SpawnPointInfo defaultSpawnPoint = new SpawnPointInfo(Vector3.zero, Quaternion.identity);


        public SpawnPointInfo(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        public static bool operator ==(SpawnPointInfo thisSpawnPoint, SpawnPointInfo spawnPoint)
        {
            bool isEqual = (thisSpawnPoint.Position - spawnPoint.Position).sqrMagnitude < 0.1f;
            isEqual &= Quaternion.Angle(thisSpawnPoint.Rotation, spawnPoint.Rotation) < 0.1f;

            return isEqual;
        }

        public static bool operator !=(SpawnPointInfo thisSpawnPoint, SpawnPointInfo spawnPoint)
        {
            bool isNotEqual = (thisSpawnPoint.Position - spawnPoint.Position).sqrMagnitude > 0.1f;
            isNotEqual |= Quaternion.Angle(thisSpawnPoint.Rotation, spawnPoint.Rotation) > 0.1f;

            return isNotEqual;
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}