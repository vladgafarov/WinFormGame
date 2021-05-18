﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TetPing
{
    class Ball
    {
        public bool IsFailed;
        private const int Size = 28;
        private int Speed = 6;
        private int MaxSpeed = 9;
        private int SpeedHorizontal;
        private int SpeedVertical;
        private Image BallTexture = Resources.Resource1.ballNew;
        private Rectangle ball;
        private int X;
        private int Y;

        public Ball()
        {
            SpeedHorizontal = Speed;
            SpeedVertical = -Speed;
            X = GameForm.Width / 2 - Size / 2;
            Y = GameForm.Height - GameForm.Height / 4;

            ball = new Rectangle(X, Y, Size, Size);
        }

        public void Draw(PaintEventArgs e)
        {
            e.Graphics.DrawImage(BallTexture, ball);
        }
        
        public void InitPhysics(Control form, Board board)
        {
            ball.X += SpeedHorizontal;
            ball.Y += SpeedVertical;

            #region walls

            //Left, Right
            if (ball.Left <= 0 || ball.Right >= GameForm.Width)
            {
                SpeedHorizontal *= -1;
            }

            //Top
            if (ball.Top <= 0)
            {
                SpeedVertical *= -1;
            }

            //Bottom
            if(ball.Bottom >= GameForm.Height)
            {
                IsFailed = true;
                ball.Location = new Point { X = X, Y = Y };
                SpeedVertical *= -1;
            }

            #endregion

            //Board
            if (ball.IntersectsWith(board.GetRect()))
            {
                SpeedVertical *= -1;
            }

            List<Block> copyBlocksList = new List<Block>(Map.BlocksList);

            copyBlocksList.ForEach(block =>
            {
                var rectBlock = block.GetRectBlock();
                var inaccuracy = 7;

                var blockBottom = rectBlock.Bottom - inaccuracy;
                var blockTop = rectBlock.Top + inaccuracy;
                var blockLeft = rectBlock.Left + inaccuracy;
                var blockRight = rectBlock.Right - inaccuracy;
                var ballBottom = ball.Bottom;
                var ballTop = ball.Top;
                var ballLeft = ball.Left;
                var ballRight = ball.Right;
                
                if (ball.IntersectsWith(rectBlock))
                {
                    #region Trajectory

                    // Trajectory
                    // To bottom right
                    if (SpeedVertical > 0 && SpeedHorizontal > 0)
                    {
                        // 1. Top board
                        if (ballBottom <= blockTop && ballRight >= blockLeft)
                            SpeedVertical *= -1;
                        // 2. Left board
                        else if (ballBottom > blockTop && ballRight < blockLeft)
                            SpeedHorizontal *= -1;
                    }
                    // To top left
                    if (SpeedVertical < 0 && SpeedHorizontal < 0)
                    {
                        // 1. Bottom board
                        if (ballTop >= blockBottom && ballLeft <= blockRight)
                            SpeedVertical *= -1;
                        // 2. Right board
                        else if (ballTop < blockBottom && ballLeft > blockRight)
                            SpeedHorizontal *= -1;
                    }

                    // To top right
                    if (SpeedVertical < 0 && SpeedHorizontal > 0)
                    {
                        // 1. Bottom board
                        if (ballTop >= blockBottom && ballRight >= blockLeft)
                            SpeedVertical *= -1;
                        // 2. Left board
                        else if(ballTop < blockBottom && ballRight < blockLeft)
                            SpeedHorizontal *= -1;
                    }
                    // To bottom left
                    if (SpeedVertical > 0 && SpeedHorizontal < 0)
                    {
                        // 1. Top board
                        if (ballBottom <= blockTop && ballLeft <= blockRight)
                            SpeedVertical *= -1;
                        // 2. Right board
                        else if (ballBottom > blockTop && ballLeft > blockRight)
                            SpeedHorizontal *= -1;
                    }
                    #endregion

                    if (block.Type == BlockType.Standart)
                        StandartAction();

                    else if (block.Type == BlockType.Ball && Balls.Count < Balls.MaxCount)
                        AddBall();

                    else if (block.Type == BlockType.BoardSpeed && Board.Speed < Board.MaxSpeed)
                        IncreaseBoardSpeed();

                    else if (block.Type == BlockType.BallSpeed && Speed < MaxSpeed)
                        IncreaseBallSpeed();

                    else if (block.Type == BlockType.Heart && Hearts.Count < Hearts.MaxCount)
                        AddHeart();

                    Score.ScoreBoard.Text = Score.score.ToString();
                    block.Remove();
                }
            });
        }

        private void StandartAction()
        {
            Score.score += 10;
        }
        private void AddBall()
        {
            Balls.AddBall();
            Score.score += 50;
        }
        private void IncreaseBoardSpeed()
        {
            Score.score += 100;
            Board.Speed += 2;
        }
        private void IncreaseBallSpeed()
        {
            Speed += 1;
            SpeedHorizontal = Speed;
            SpeedVertical = Speed;
            Score.score += 50;
        }
        private void AddHeart()
        {
            Hearts.AddHeart();
            Score.score += 50;
        }
    }
}
