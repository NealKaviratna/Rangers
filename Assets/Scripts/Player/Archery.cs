﻿using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Timers;
using Assets.Scripts.Tokens;
using Assets.Scripts.Util;

/*
 * This class handles all the shooting that can be done by actors
 */
namespace Assets.Scripts.Player
{
	public class Archery : ControllerObject
	{
        [SerializeField]
        private GameObject arrowPrefab;
        [SerializeField]
        private Transform firePoint;

        // All the token timers running
        private List<TokenTimer> timers;

        // The current types of arrows to fire
        private int types = 1;

        void Awake()
        {
            timers = new List<TokenTimer>();
        }

        // Fires an arrow
        public void Fire()
        {
            GameObject arrow = (GameObject)Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            arrow.GetComponent<Rigidbody>().AddRelativeForce(arrow.transform.right * 10, ForceMode.Impulse);
            arrow.GetComponent<Arrows.ArrowController>().InitArrow(types);
        }

        // Called by colliding with a Token (from Token's OnTriggerEnter)
        public override void CollectToken(Token token)
        {
            // Handle what type of token was collected
            if (token.GetType().Equals(typeof(ArrowToken)))
            {
                // Find the running timer associated with the token
                TokenTimer t = timers.Find(i => i.ID.Equals(((ArrowToken)token).Type.ToString()));
                // If the token has not been collected yet
                if (t == null)
                {
                    // Add a new Token Timer and initialize it
                    TokenTimer tt = gameObject.AddComponent<TokenTimer>();
                    tt.Initialize(TokenTimer.TOKEN_INTERVAL, ((ArrowToken)token).Type.ToString());
                    // Make sure that the token is removed from the component when the timer times out
                    tt.TimeOut += new TokenTimer.TimerEvent(RemoveToken);
                    types = Bitwise.SetBit(types, (int)tt.TokenType);
                    timers.Add(tt);
                }
                else
                {
                    // Token has already been collected so we just need to reset the timer
                    t.Reset();
                }
            }
        }

        // Removes the token from the types the player has collected
        private void RemoveToken(TokenTimer tt)
        {
            // Clear the appropriate token bit and remove the timer from the list of running timers
            types = Bitwise.ClearBit(types, (int)tt.TokenType);
            TokenTimer t = timers.Find(i => i.ID.Equals(tt.TokenType.ToString()));
            timers.Remove(t);
        }
	}
}