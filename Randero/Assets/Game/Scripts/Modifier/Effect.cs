namespace Randero.Modifier
{
    [System.Serializable]
    public struct Effect
    {
        public float damage;
        public float continuingDamagePercentage;
        public int continuingDamageTurns;

        public bool doesFreeze;
    }
}

