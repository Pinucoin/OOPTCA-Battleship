using Common;
using System.Linq;

namespace DataAccess
{
    public class AttackRepository : ConnectionContext
    {
        public IQueryable<Attack> GetAttacksByPlayerId(int playerId)
        {
            var attacksList = from Attack in Context.Attacks
                              where Attack.playerFK == playerId
                              select Attack;
            return attacksList.AsQueryable();
        }

        public IQueryable<Attack> GetAttackHitsByPlayerId(int playerId)
        {
            var hitList = from Attack in Context.Attacks
                          where Attack.playerFK == playerId
                          where Attack.hit == true
                          select Attack;
            return hitList.AsQueryable();
        }

        public void addAttack(Attack attack)
        {
            Context.Attacks.Add(attack);
            Context.SaveChanges();
        }
    }
}
