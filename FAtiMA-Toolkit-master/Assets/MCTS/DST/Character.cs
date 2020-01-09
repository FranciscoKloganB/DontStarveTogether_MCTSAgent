using System;
using Utilities;

namespace MCTS.DST.WorldModels
{

    public class Character
    {
        private const float MIN_ANY = 0.0f;
        private const float MAX_SATIATION = 150.0f;
        private const float MAX_HP = 150.0f;
        private const float MAX_SANITY = 200.0f;

        public float HP;
        public float Satiation;
        public float Sanity;
        public Pair<int, int> Position;

        public Character()
        {
        }

        public Character(float hp, float satiation, float sanity, int posx, int posz)
        {
            this.HP = hp;
            this.Satiation = satiation;
            this.Sanity = sanity;
            this.Position = new Pair<int, int>(posx, posz);
        }

        public int GetPosX()
        {
            return Position.Item1;
        }

        public int GetPosZ()
        {
            return Position.Item2;
        }

        public void UpdateSatiation(float value)
        {
            this.Satiation = Clamp(this.Satiation + value, MIN_ANY, MAX_SATIATION);
        }

        public void UpdateHP(float value)
        {
            this.HP = Clamp(this.HP + value, MIN_ANY, MAX_HP);
        }

        public void UpdateSanity(float value)
        {
            this.Sanity = Clamp(this.Sanity + value, MIN_ANY, MAX_SANITY);
        }

        private float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    } 
}
