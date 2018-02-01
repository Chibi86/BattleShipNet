using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Player
    {
        public string Name { get; set; }
        public List<Boat> Boats { get; }
        public List<Position> AlreadyHitPositions { get; private set; }
        public bool HaveSeenEndScreen{ get; set; }

        /// <summary>
        /// Properties for return if player has lost game - get
        /// </summary>
        public bool HasPlayerLost
        {
            get
            {
                return (Boats.Find(boat => boat.Sink == false) == null);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Player()
        {
            AlreadyHitPositions = new List<Position>();
            Name = null;
            HaveSeenEndScreen = false;

            Boats = new List<Boat> {
                new Boat(BoatType.Battleship),
                new Boat(BoatType.Cruiser),
                new Boat(BoatType.Destroyer),
                new Boat(BoatType.Destroyer),
                new Boat(BoatType.Submarine),
                new Boat(BoatType.Submarine),
                new Boat(BoatType.Submarine)
            };

            PositionsBoats();
        }

        /// <summary>
        /// Positions Boats random, based on https://github.com/exceptionnotfound/BattleshipModellingPractice/
        /// </summary>
        private void PositionsBoats()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            foreach (Boat boat in Boats)
            {
                // Select a random row/column combination, then select a random orientation.
                // If none of the proposed squares are occupied, place the Boat
                // Do this for all Boats

                Position[] positions = new Position[2];

                while (boat.Positions[0] == null || boat.Positions[1] == null)
                {
                    int startX = rand.Next(1, 11);
                    int startY = rand.Next(1, 11);
                    int endY = startY;
                    int endX = startX;

                    int orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                    // Calculate endY (Horizontal) or endX (Vertical)
                    if (orientation == 0)
                    {
                        endY += boat.Size - 1;
                    }
                    else
                    {
                        endX += boat.Size - 1;
                    }

                    //We cannot place Boats beyond the boundaries of the board
                    try
                    {
                        positions[0] = new Position(startX, startY);
                        positions[1] = new Position(endX, endY);
                    }
                    catch
                    {
                        continue;
                    }

                    bool test = IsAnyBoatHere(positions);

                    // Check so positions is free to move boat to
                    if (!test)
                    {
                        boat.SetPositions(positions);
                    }
                }
            }
        }

        /// <summary>
        /// Check positions for Boats and hits
        /// </summary>
        /// <returns>Square object with data</returns>
        public Square CheckPosition(Position position)
        {
            Square square = new Square();

            if (IsAnyBoatHere(position))
            {
                square.HaveBoat = true;
            }

            if (IsPositionAlreadyHit(position))
            {
                square.HaveBeenHit = true;
            }

            return square;
        }

        /// <summary>
        /// Is position already hit
        /// </summary>
        /// <param name="position">Position to hit (Position)</param>
        /// <returns>Validate result (bool)</returns>
        public bool IsPositionAlreadyHit(Position position)
        {
            foreach (Position hitPosition in AlreadyHitPositions)
            {
                if (position.X == hitPosition.X && position.Y == hitPosition.Y)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if any Boat was hit
        /// </summary>
        /// <param name="position">Position to hit (Position)</param>
        /// <returns>Validate result (bool)</returns>
        public bool IsABoatHit(Position position)
        {
            if (IsPositionAlreadyHit(position))
            {
                throw new Exception("Position is already hit, try other position!");
            }
            else
            {
                AlreadyHitPositions.Add(position);

                foreach (var boat in Boats)
                {
                    if (boat.IsItAHit(position))
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Is positions free to move a Boat object to?
        /// </summary>
        /// <param name="positions">Positions to check (Position[])</param>
        /// <returns>Validate result (bool)</returns>
        public bool IsAnyBoatHere(Position[] positions)
        {
            //Check if specified squad are occupied
            foreach (Boat boat in Boats)
            {
                if (boat.AreYouHere(positions))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Do positions got any Boat object?
        /// </summary>
        /// <param name="position">Position to check (Position)</param>
        /// <returns>Validate result (bool)</returns>
        public bool IsAnyBoatHere(Position position)
        {
            //Check if specified squad are occupied
            foreach (Boat boat in Boats)
            {
                if (boat.AreYouHere(position))
                {
                    return true;
                }
            }

            return false;
        }
    }
}