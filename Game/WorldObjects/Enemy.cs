using System.Numerics;
using PixelArtGameJam.Game.Abilities;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.WorldObjects
{
    public class Enemy : WorldObject
    {
        Dungeon dungeonReference { get; set; }

        int attackRange { get; set; } //Attack range in grid spaces
        int moveSpeed { get; set; } //How many moves the enemy can make per turn
        bool hasAttackedThisTurn { get; set; } = false;

        Ability ability { get; set; }

        int maxHealth { get; set; }
        int currentHealth { get; set; }
        int stunFactor { get; set; }
        int fearFactor { get; set; }
        int burnFactor { get; set; }

        public Enemy(Vector2 position, ObjectType objType, Dungeon dungeonRef, int maxHealth, int attackRange, int moveSpeed, Ability ability) : base(position, objType)
        {
            this.dungeonReference = dungeonRef;
            this.attackRange = attackRange;
            this.moveSpeed = moveSpeed;
            this.ability = ability;

            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.stunFactor = 0;
            this.fearFactor = 0;
            this.burnFactor = 0;
        }

        private void PerformAction() //Add if distance is 1 then enemy backs off
        {
            if (hasAttackedThisTurn)
            {
                return;
            }

            //Return Vector2 calculating the direction of player to enemy position
            Vector2 directionToPlayer = GetDirectionToPlayer();

            //Check if enemy and player are in alignment in 2d grid space
            if (directionToPlayer.X == 0 || directionToPlayer.Y == 0)
            {
                //If enemy is in attack range shoot projectile
                if (IsPlayerInAttackRange() && fearFactor == 0)
                {
                    AttackPlayer(directionToPlayer);
                }

                //Else move toward player
                else
                {
                    MoveTowardPlayer(directionToPlayer);
                }
            }

            //If neither x or y is 0 in directionToPlayer, calculate a random direction
            else
            {
                Vector2 directionToMove = GetDirectionToMove(directionToPlayer);
                MoveTowardPlayer(directionToMove);
            }
            
        }

        private float CalculateDistanceToPlayer()
        {
            Player player = dungeonReference.player;
            float distance = Vector2.Distance(position, player.position) / dungeonReference.map.gridSize;
            return distance;
        }

        private bool IsPlayerInAttackRange()
        {
            float distance = CalculateDistanceToPlayer();

            if (distance > attackRange)
            {
                return false;
            }
            else if (distance <= 1) //Enemy moves away from player if too close
            {
                fearFactor = 1;
            }
                        
            return true;
        }

        private Vector2 GetDirectionToPlayer()
        {
            Player player = dungeonReference.player;

            Vector2 playerPos = player.position;
            Vector2 playerGridPos = playerPos / dungeonReference.map.gridSize;
            Vector2 enemyGridPos = position / dungeonReference.map.gridSize;

            int projectedX = 0;
            int projectedY = 0;

            if (enemyGridPos.X != playerGridPos.X)
            {
                if (enemyGridPos.X > playerGridPos.X)
                {
                    projectedX = -1;
                }
                else
                {
                    projectedX = 1;
                }
            }

            if (enemyGridPos.Y != playerGridPos.Y)
            {
                if (enemyGridPos.Y > playerGridPos.Y)
                {
                    projectedY = -1;
                }
                else
                {
                    projectedY = 1;
                }
            }

            return new Vector2(projectedX, projectedY);
        }

        private Vector2 GetDirectionToMove(Vector2 playerDirection)
        { 
            Vector2 direction = Vector2.Zero;
                        
            Random random = new Random();
            int randNumb = random.Next(1, 3);

            switch (randNumb)
            {
                case 1:
                    direction = new Vector2(playerDirection.X, 0);
                    break;
                case 2:
                    direction = new Vector2(0, playerDirection.Y);
                    break;
            }
            
            return direction;
        }
        
        private void MoveTowardPlayer(Vector2 direction)
        {           

            if (fearFactor > 0)
            {
                fearFactor--;
                direction *= -1;
            }

            
            Vector2 newPosition = dungeonReference.MoveWorldObject(this, position, direction);
            UpdatePosition(newPosition);
            
            
        }


        private void AttackPlayer(Vector2 direction) //Fix me: Check if grid space is empty before casting?
        {
            float gridX = (float)Math.Floor(position.X / dungeonReference.map.gridSize);
            float gridY = (float)Math.Floor(position.Y / dungeonReference.map.gridSize);

            Vector2 gridPos = new Vector2(gridX, gridY);

            

                Vector2 targetGridPos = gridPos + direction;

            if (dungeonReference.TestGridSpaceEmpty(targetGridPos))
            {
                double angle = Math.Atan2(direction.Y, -direction.X);
                angle = angle * 180 / Math.PI * -1;

                if (angle == -180)
                {
                    angle = 180;
                }

                ability.Cast(dungeonReference, targetGridPos, (float)angle);
                hasAttackedThisTurn = true;
            }
        }

        public override void ResolveDamage(int damage, int stunFactor, int fearFactor, int burnFactor)
        {
            currentHealth -= damage;
            this.stunFactor += stunFactor;
            this.fearFactor += fearFactor;
            this.burnFactor += burnFactor;

            if (currentHealth <= 0)
            {
                dungeonReference.RemoveObjectFromDungeon(this, position);
                dungeonReference.RemoveEnemyFromEnemies(this);
            }
        }

        public void EnvironmentUpdate()
        {
            hasAttackedThisTurn = false; //Reset bool - prevents enemies with 2 movespeed attacking more than once

            //Stun factor causes enemy to skip turns.
            //Subtract 1 from stunfactor if > 0 else performa action
            if (stunFactor > 0)
            {
                stunFactor--;
            }
            else
            {
                for (int i = 0; i < moveSpeed; i++)
                {
                    PerformAction();
                }
                
            }


            if (burnFactor > 0)
            {
                burnFactor--;
                ResolveDamage(5, 0, 0, 0);
            }
        }

    }
}
