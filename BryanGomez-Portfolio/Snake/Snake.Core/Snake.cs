using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake.Core
{
    public class Snake
    {
        public enum Direction { Up, Down, Left, Right };

        // for the snake inital body and direction
        private Direction m_currentDirection = Direction.Right;
        private readonly List<Point> m_body;
        private bool m_IsGrowing;

        // These are the "Smooth Movement" helpers
        // using 'public' so the Draw method in the Game class can see them
        public Direction PreviousDirection { get; private set; }
        public List<Point> OldFullBody { get; private set; }
        public List<Point> FullBody => m_body;

        #region Constructor
        public Snake(int startX, int startY)
        {
            m_body = new List<Point>();
            m_body.Add(new Point(startX, startY));
            m_body.Add(new Point(startX - 1, startY));
            m_body.Add(new Point(startX - 2, startY));

            OldFullBody = [.. m_body];
            PreviousDirection = Direction.Right;
            m_currentDirection = Direction.Right;
        }
        #endregion

        #region Properties
        public Point Head => m_body[0];
        public IEnumerable<Point> Body => m_body.Skip(1);

        public Direction CurrentDirection => m_currentDirection;
        #endregion

        #region Methods
        public void Move()
        {
            // Capturing state for smooth interpolation BEFORE changing the body
            OldFullBody = [.. m_body];
            PreviousDirection = m_currentDirection;

            Point newHead = Head;

            switch (m_currentDirection)
            {
                case Direction.Up: newHead.Y -= 1; break;
                case Direction.Down: newHead.Y += 1; break;
                case Direction.Left: newHead.X -= 1; break;
                case Direction.Right: newHead.X += 1; break;
                default: throw new InvalidOperationException("Invalid direction");
            }

            m_body.Insert(0, newHead);

            if (!m_IsGrowing)
            {
                m_body.RemoveAt(m_body.Count - 1);
            }
            else
            {
                OldFullBody.Add(OldFullBody[^1]);
                m_IsGrowing = false;
            }
        }

        public void SetDirection(Direction newDirection)
        {
            // Preventing 180-degree turns
            if (newDirection == Direction.Up && m_currentDirection == Direction.Down ||
                newDirection == Direction.Down && m_currentDirection == Direction.Up ||
                newDirection == Direction.Left && m_currentDirection == Direction.Right ||
                newDirection == Direction.Right && m_currentDirection == Direction.Left)
            {
                return;
            }
            m_currentDirection = newDirection;
        }

        public bool CheckSelfCollision()
        {
            foreach (Point bodyPart in Body)
            {
                if (Head == bodyPart) return true;
            }
            return false;
        }

        public bool CheckWallCollision(GameBoard board)
        {
            if (Head.X < 0 || Head.X >= GameBoard.GRID_WIDTH ||
                Head.Y < 0 || Head.Y >= GameBoard.GRID_HEIGHT)
            {
                return true;
            }
            return false;
        }

        public void Grow() => m_IsGrowing = true;
        #endregion
    }
}