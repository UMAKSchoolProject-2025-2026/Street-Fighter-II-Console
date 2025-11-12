using System;
using System.Collections.Generic;
using StreetFighter2.Fighters;

namespace StreetFighter2.Combat
{
    public class CombatResolver
    {
        private Random random;

        public CombatResolver()
        {
            random = new Random();
        }

        public TurnResult ResolveTurn(Fighter attacker, Fighter defender, Move attackerMove, Move defenderMove, string attackerName, string defenderName)
        {
            TurnResult result = new TurnResult();
            bool attackerIsPlayer1 = attackerName.Contains("1") || attackerName == "P1";
            bool attackerSpecialFailed = false;
            bool defenderSpecialFailed = false;
            Move validatedAttackerMove = attackerMove;
            Move validatedDefenderMove = defenderMove;

            // ============================================================
            // POLYMORPHISM: Using CanUseSpecialMove() - each fighter can override
            // ============================================================
            if (attackerMove == Move.Special && !attacker.CanUseSpecialMove())
            {
                result.AddLog($"{attackerName} tried to use their special move, but didn't have enough SP!");
                result.AddLog($"{attackerName} is left wide open!");
                attackerSpecialFailed = true;
                validatedAttackerMove = Move.Block;
            }

            if (defenderMove == Move.Special && !defender.CanUseSpecialMove())
            {
                result.AddLog($"{defenderName} tried to use their special move, but didn't have enough SP!");
                result.AddLog($"{defenderName} is left wide open!");
                defenderSpecialFailed = true;
                validatedDefenderMove = Move.Block;
            }

            bool attackerHasValidSpecial = validatedAttackerMove == Move.Special && !attackerSpecialFailed;
            bool defenderHasValidSpecial = validatedDefenderMove == Move.Special && !defenderSpecialFailed;

            if (attackerHasValidSpecial && defenderHasValidSpecial)
            {
                // ============================================================
                // POLYMORPHISM: Calling GetSpecialMoveName() - each fighter returns different name
                // ============================================================
                string attackerSpecial = attacker.GetSpecialMoveName();
                string defenderSpecial = defender.GetSpecialMoveName();
                result.AddLog($"{attackerName} unleashes {attackerSpecial}!");
                result.AddLog($"{defenderName} counters with {defenderSpecial}!");
                result.AddLog("The special moves collide in a spectacular explosion!");
                result.AddLog("Forces cancel each other out!");
                result.Cancelled = true;
                return result;
            }

            if (attackerHasValidSpecial)
            {
                ApplySpecial(attacker, defender, attackerName, defenderName, result, attackerIsPlayer1);
                return result;
            }

            if (defenderHasValidSpecial)
            {
                ApplySpecial(defender, attacker, defenderName, attackerName, result, !attackerIsPlayer1);
                return result;
            }

            if (validatedAttackerMove == validatedDefenderMove && 
                (validatedAttackerMove == Move.LightAttack || validatedAttackerMove == Move.HeavyAttack))
            {
                string moveName = validatedAttackerMove == Move.LightAttack ? "Light Attack" : "Heavy Attack";
                result.AddLog($"{attackerName} uses {moveName}!");
                result.AddLog($"{defenderName} uses {moveName}!");
                result.AddLog("Both attacks clash!");
                result.AddLog("Both fighters take reduced damage from the impact!");
                
                // Apply 50% damage to both fighters when same attacks collide
                int attackerDamage = validatedAttackerMove == Move.LightAttack ? attacker.AttackPower : attacker.AttackPower * 2;
                int defenderDamage = validatedDefenderMove == Move.LightAttack ? defender.AttackPower : defender.AttackPower * 2;
                
                int reducedAttackerDamage = attackerDamage / 2;
                int reducedDefenderDamage = defenderDamage / 2;
                
                // Apply damage to both
                defender.CurrentHealth -= reducedAttackerDamage;
                attacker.CurrentHealth -= reducedDefenderDamage;
                
                // Call polymorphic methods
                attacker.OnAttackLanded(defender, reducedAttackerDamage);
                defender.OnDamageTaken(attacker, reducedAttackerDamage);
                
                defender.OnAttackLanded(attacker, reducedDefenderDamage);
                attacker.OnDamageTaken(defender, reducedDefenderDamage);
                
                // Track damage for UI
                if (attackerIsPlayer1)
                {
                    result.DamageToPlayer2 += reducedAttackerDamage;
                    result.DamageToPlayer1 += reducedDefenderDamage;
                }
                else
                {
                    result.DamageToPlayer1 += reducedAttackerDamage;
                    result.DamageToPlayer2 += reducedDefenderDamage;
                }
                
                return result;
            }

            DescribeInteraction(validatedAttackerMove, validatedDefenderMove, attackerName, defenderName, result);
            bool attackerHitsFirst = DetermineInitiative(validatedAttackerMove, validatedDefenderMove);

            if (attackerHitsFirst)
            {
                if (defenderSpecialFailed)
                {
                    ApplyMoveIgnoreDefense(attacker, defender, validatedAttackerMove, attackerName, defenderName, result, attackerIsPlayer1);
                }
                else
                {
                    ApplyMove(attacker, defender, validatedAttackerMove, validatedDefenderMove, attackerName, defenderName, result, attackerIsPlayer1);
                }

                if (defender.CurrentHealth > 0 && validatedDefenderMove != Move.Block && validatedDefenderMove != Move.Dodge)
                {
                    if (attackerSpecialFailed)
                    {
                        ApplyMoveIgnoreDefense(defender, attacker, validatedDefenderMove, defenderName, attackerName, result, !attackerIsPlayer1);
                    }
                    else
                    {
                        ApplyMove(defender, attacker, validatedDefenderMove, validatedAttackerMove, defenderName, attackerName, result, !attackerIsPlayer1);
                    }
                }
            }
            else
            {
                if (attackerSpecialFailed)
                {
                    ApplyMoveIgnoreDefense(defender, attacker, validatedDefenderMove, defenderName, attackerName, result, !attackerIsPlayer1);
                }
                else
                {
                    ApplyMove(defender, attacker, validatedDefenderMove, validatedAttackerMove, defenderName, attackerName, result, !attackerIsPlayer1);
                }

                if (attacker.CurrentHealth > 0 && validatedAttackerMove != Move.Block && validatedAttackerMove != Move.Dodge)
                {
                    if (defenderSpecialFailed)
                    {
                        ApplyMoveIgnoreDefense(attacker, defender, validatedAttackerMove, attackerName, defenderName, result, attackerIsPlayer1);
                    }
                    else
                    {
                        ApplyMove(attacker, defender, validatedAttackerMove, validatedDefenderMove, attackerName, defenderName, result, attackerIsPlayer1);
                    }
                }
            }

            return result;
        }

        private void ApplyMoveIgnoreDefense(Fighter attacker, Fighter defender, Move attackerMove, string attackerName, string defenderName, TurnResult result, bool attackerIsPlayer1)
        {
            int damage = 0;

            if (attackerMove == Move.LightAttack)
            {
                damage = attacker.AttackPower;
            }
            else if (attackerMove == Move.HeavyAttack)
            {
                damage = attacker.AttackPower * 2;
            }

            if (damage > 0)
            {
                defender.CurrentHealth -= damage;
                
                // ============================================================
                // POLYMORPHISM: Calling OnAttackLanded - each fighter can override behavior
                // ============================================================
                attacker.OnAttackLanded(defender, damage);
                
                // ============================================================
                // POLYMORPHISM: Calling OnDamageTaken - each fighter can override behavior
                // ============================================================
                defender.OnDamageTaken(attacker, damage);

                if (attackerIsPlayer1)
                    result.DamageToPlayer2 += damage;
                else
                    result.DamageToPlayer1 += damage;
            }
        }

        private void DescribeInteraction(Move playerMove, Move enemyMove, string playerName, string enemyName, TurnResult result)
        {
            if (playerMove == Move.LightAttack && enemyMove == Move.LightAttack)
            {
                result.AddLog($"{playerName} and {enemyName} exchange rapid jabs!");
                result.AddLog("Both fighters land their attacks simultaneously!");
                return;
            }

            if (playerMove == Move.LightAttack && enemyMove == Move.HeavyAttack)
            {
                result.AddLog($"{playerName} throws a quick jab!");
                result.AddLog($"{enemyName} winds up for a heavy strike...");
                result.AddLog($"But {playerName}'s lightning-fast attack interrupts it!");
                return;
            }
            if (playerMove == Move.HeavyAttack && enemyMove == Move.LightAttack)
            {
                result.AddLog($"{playerName} prepares a devastating blow!");
                result.AddLog($"{enemyName} strikes with blinding speed!");
                result.AddLog($"{playerName}'s attack is stopped before it connects!");
                return;
            }

            if (playerMove == Move.LightAttack && enemyMove == Move.Block)
            {
                result.AddLog($"{playerName} launches a quick strike!");
                result.AddLog($"{enemyName} raises their guard and blocks it completely!");
                return;
            }
            if (playerMove == Move.Block && enemyMove == Move.LightAttack)
            {
                result.AddLog($"{enemyName} throws a quick punch!");
                result.AddLog($"{playerName} blocks it with perfect timing!");
                return;
            }

            if (playerMove == Move.LightAttack && enemyMove == Move.Dodge)
            {
                result.AddLog($"{playerName} throws a swift attack!");
                result.AddLog($"{enemyName} tries to evade...");
                result.AddLog($"But the attack is too fast!");
                return;
            }
            if (playerMove == Move.Dodge && enemyMove == Move.LightAttack)
            {
                result.AddLog($"{enemyName} launches a quick strike!");
                result.AddLog($"{playerName} attempts to dodge...");
                result.AddLog($"But can't escape in time!");
                return;
            }

            if (playerMove == Move.HeavyAttack && enemyMove == Move.HeavyAttack)
            {
                result.AddLog($"{playerName} and {enemyName} both wind up massive strikes!");
                result.AddLog("CRASH! Both heavy attacks collide with tremendous force!");
                result.AddLog("Both fighters stagger from the impact!");
                return;
            }

            if (playerMove == Move.HeavyAttack && enemyMove == Move.Block)
            {
                result.AddLog($"{playerName} unleashes a powerful heavy attack!");
                result.AddLog($"{enemyName} tries to block...");
                result.AddLog("But the force is overwhelming! The attack breaks through!");
                return;
            }
            if (playerMove == Move.Block && enemyMove == Move.HeavyAttack)
            {
                result.AddLog($"{enemyName} charges a devastating blow!");
                result.AddLog($"{playerName} braces for impact...");
                result.AddLog("The attack shatters through the defense!");
                return;
            }

            if (playerMove == Move.HeavyAttack && enemyMove == Move.Dodge)
            {
                result.AddLog($"{playerName} winds up a massive strike!");
                result.AddLog($"{enemyName} reads the attack and evades gracefully!");
                result.AddLog($"{playerName}'s attack whiffs through empty air!");
                return;
            }
            if (playerMove == Move.Dodge && enemyMove == Move.HeavyAttack)
            {
                result.AddLog($"{enemyName} charges a powerful attack!");
                result.AddLog($"{playerName} anticipates and dodges at the last moment!");
                result.AddLog("The attack misses completely!");
                return;
            }

            if (playerMove == Move.Block && enemyMove == Move.Block)
            {
                result.AddLog("Both fighters stand in defensive stance...");
                result.AddLog("A tense standoff as they wait for an opening!");
                return;
            }

            if (playerMove == Move.Dodge && enemyMove == Move.Dodge)
            {
                result.AddLog("Both fighters are moving evasively!");
                result.AddLog("They circle each other, looking for an advantage!");
                return;
            }

            if ((playerMove == Move.Block && enemyMove == Move.Dodge) || 
                (playerMove == Move.Dodge && enemyMove == Move.Block))
            {
                result.AddLog("Both fighters play it safe with defensive maneuvers!");
                result.AddLog("No attacks connect this round!");
                return;
            }
        }

        private bool DetermineInitiative(Move move1, Move move2)
        {
            if (move1 == Move.LightAttack && move2 == Move.HeavyAttack) return true;
            if (move2 == Move.LightAttack && move1 == Move.HeavyAttack) return false;
            return random.Next(0, 2) == 0;
        }

        private void ApplyMove(Fighter attacker, Fighter defender, Move attackerMove, Move defenderMove, string attackerName, string defenderName, TurnResult result, bool attackerIsPlayer1)
        {
            switch (attackerMove)
            {
                case Move.LightAttack:
                    ApplyLightAttack(attacker, defender, defenderMove, attackerName, defenderName, result, attackerIsPlayer1);
                    break;

                case Move.HeavyAttack:
                    ApplyHeavyAttack(attacker, defender, defenderMove, attackerName, defenderName, result, attackerIsPlayer1);
                    break;

                case Move.Block:
                case Move.Dodge:
                    break;
            }
        }

        private void ApplyLightAttack(Fighter attacker, Fighter defender, Move defenderMove, string attackerName, string defenderName, TurnResult result, bool attackerIsPlayer1)
        {
            int baseDamage = attacker.AttackPower;

            // ============================================================
            // POLYMORPHISM: Using CalculateDamageMultiplier - each fighter has unique calculation
            // ============================================================
            int multiplier = attacker.CalculateDamageMultiplier(Move.LightAttack, defenderMove);
            int damage = (baseDamage * multiplier) / 100;

            if (defenderMove == Move.Block)
            {
                damage = 0;
            }

            if (damage > 0)
            {
                defender.CurrentHealth -= damage;
                
                // ============================================================
                // POLYMORPHISM: Calling OnAttackLanded - each fighter can override behavior
                // ============================================================
                attacker.OnAttackLanded(defender, damage);
                
                // ============================================================
                // POLYMORPHISM: Calling OnDamageTaken - each fighter can override behavior
                // ============================================================
                defender.OnDamageTaken(attacker, damage);

                if (attackerIsPlayer1)
                    result.DamageToPlayer2 += damage;
                else
                    result.DamageToPlayer1 += damage;
            }
        }

        private void ApplyHeavyAttack(Fighter attacker, Fighter defender, Move defenderMove, string attackerName, string defenderName, TurnResult result, bool attackerIsPlayer1)
        {
            int baseDamage = attacker.AttackPower * 2;

            // ============================================================
            // POLYMORPHISM: Using CalculateDamageMultiplier - each fighter has unique calculation
            // ============================================================
            int multiplier = attacker.CalculateDamageMultiplier(Move.HeavyAttack, defenderMove);
            int damage = (baseDamage * multiplier) / 100;

            if (defenderMove == Move.LightAttack)
            {
                damage = 0;
            }

            if (defenderMove == Move.Dodge)
            {
                damage = 0;
            }

            if (damage > 0)
            {
                defender.CurrentHealth -= damage;
                
                // ============================================================
                // POLYMORPHISM: Calling OnAttackLanded - each fighter can override behavior
                // ============================================================
                attacker.OnAttackLanded(defender, damage);
                
                // ============================================================
                // POLYMORPHISM: Calling OnDamageTaken - each fighter can override behavior
                // ============================================================
                defender.OnDamageTaken(attacker, damage);

                if (attackerIsPlayer1)
                    result.DamageToPlayer2 += damage;
                else
                    result.DamageToPlayer1 += damage;
            }
        }

        private void ApplySpecial(Fighter attacker, Fighter defender, string attackerName, string defenderName, TurnResult result, bool attackerIsPlayer1)
        {
            // ============================================================
            // POLYMORPHISM: Calling GetSpecialMoveName - each fighter returns different name
            // ============================================================
            string specialName = attacker.GetSpecialMoveName();
            
            // ============================================================
            // POLYMORPHISM: Calling GetSpecialMoveDescription - each fighter returns different description
            // ============================================================
            string description = attacker.GetSpecialMoveDescription();
            
            int baseDamage = attacker.SpecialMovePower;

            // ============================================================
            // POLYMORPHISM: Using CalculateDamageMultiplier for special moves
            // ============================================================
            int multiplier = attacker.CalculateDamageMultiplier(Move.Special, Move.LightAttack);
            int damage = (baseDamage * multiplier) / 100;

            defender.CurrentHealth -= damage;
            attacker.SpBar -= attacker.SpecialMoveCost;

            result.AddLog($"{attackerName} unleashes {specialName}!");
            result.AddLog(description);
            result.AddLog($"CRITICAL HIT! {damage} damage!");

            // ============================================================
            // POLYMORPHISM: Calling OnAttackLanded - each fighter can override behavior
            // ============================================================
            attacker.OnAttackLanded(defender, damage);
            
            // ============================================================
            // POLYMORPHISM: Calling OnDamageTaken - each fighter can override behavior
            // ============================================================
            defender.OnDamageTaken(attacker, damage);

            if (attackerIsPlayer1)
                result.DamageToPlayer2 += damage;
            else
                result.DamageToPlayer1 += damage;
        }
    }
}