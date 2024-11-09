using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Room object movement provider.
    /// </summary>
    public abstract class RoomObjectMovementProvider
    {
        /// <summary>
        /// Object being moved.
        /// </summary>
        public RoomObject RoomObject { get; private set; }

        /// <summary>
        /// Private status.
        /// </summary>
        protected Vector2 m_Position;
        protected Vector2 m_Velocity;

        /// <summary>
        /// Room accessors.
        /// </summary>
        protected Room Room { get { return this.RoomObject.Room; } }
        protected TileGrid ActiveTileGrid { get { return this.Room.ActiveTileGrid; } }
        protected Size ActiveTileGridTileSize { get { return this.ActiveTileGrid.TileSize; } }
        protected TerrainList Terrains { get { return this.Room.Terrains; } }
        protected RoomObjectList Obstacles { get { return this.Room.Obstacles; } }
        protected MainCharacter MainCharacter { get { return this.Room.MainCharacter; } }

        /// <summary>
        /// Movement conditions.
        /// </summary>
        protected GameConditionList Conditions { get; private set; }

        #region Size and Position helpers

        /// <summary>
        /// Size helpers.
        /// </summary>
        public float Width { get { return this.RoomObject.Size.X; } }
        public float Height { get { return this.RoomObject.Size.Y; } }

        /// <summary>
        /// Position helpers.
        /// </summary>
        public float Left { get { return m_Position.X; } }
        public float Top { get { return m_Position.Y; } }
        public float Right { get { return m_Position.X + this.RoomObject.Size.X; } }
        public float Bottom { get { return m_Position.Y + this.RoomObject.Size.Y; } }
        public float MidX { get { return m_Position.X + (this.RoomObject.Size.X / 2); } }
        public float MidY { get { return m_Position.Y + (this.RoomObject.Size.Y / 2); } }
        public Vector2 Mid { get { return new Vector2(m_Position.X + (this.RoomObject.Size.X / 2), m_Position.Y + (this.RoomObject.Size.Y / 2)); } }

        /// <summary>
        /// Previous position helpers.
        /// </summary>
        public float PreviousLeft { get { return this.RoomObject.PreviousPosition.X; } }
        public float PreviousTop { get { return this.RoomObject.PreviousPosition.Y; } }
        public float PreviousRight { get { return this.RoomObject.PreviousPosition.X + this.RoomObject.Size.X; } }
        public float PreviousBottom { get { return this.RoomObject.PreviousPosition.Y + this.RoomObject.Size.Y; } }
        public float PreviousMidX { get { return this.RoomObject.PreviousPosition.X + (this.RoomObject.Size.X / 2); } }
        public float PreviousMidY { get { return this.RoomObject.PreviousPosition.Y + (this.RoomObject.Size.Y / 2); } }
        public Vector2 PreviousMid { get { return new Vector2(this.RoomObject.PreviousPosition.X + (this.RoomObject.Size.X / 2), this.RoomObject.PreviousPosition.Y + (this.RoomObject.Size.Y / 2)); } }

        #endregion

        /// <summary>
        /// State.
        /// </summary>
        public string ActionState { get; protected set; }

        /// <summary>
        /// State of surrounding environment.
        /// </summary>
        protected SurfaceInformation LeftSide { get; private set; }
        protected SurfaceInformation TopSide { get; private set; }
        protected SurfaceInformation RightSide { get; private set; }
        protected SurfaceInformation BottomSide { get; private set; }
        protected Terrain CollidedTerrain { get; private set; }
        protected Vector2 ActiveTileAcceleration { get; private set; }
        protected float ActiveTileAccelerationLength { get; private set; }
        protected bool IsGroundInFrontOfBaseSolid { get; set; }

        protected virtual float GravityShift { get { return 0f; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObjectMovementProvider(GameConditionList p_Conditions)
        {
            this.Conditions = new GameConditionList();
            if (p_Conditions != null)
            {
                for (int i = 0; i < p_Conditions.Count; i++)
                {
                    this.Conditions.Add(p_Conditions[i]);
                }
            }

            this.LeftSide = new SurfaceInformation(SurfaceDirections.Left);
            this.RightSide = new SurfaceInformation(SurfaceDirections.Right);
            this.TopSide = new SurfaceInformation(SurfaceDirections.Top);
            this.BottomSide = new SurfaceInformation(SurfaceDirections.Bottom);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(RoomObject p_RoomObject)
        {
            this.RoomObject = p_RoomObject;
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset(Vector2 p_Position, Vector2 p_Velocity)
        {
            this.LeftSide.Reset();
            this.RightSide.Reset();
            this.TopSide.Reset();
            this.BottomSide.Reset();
            this.IsGroundInFrontOfBaseSolid = false;

            // Forget previous tile acceleration
            this.ActiveTileAcceleration = Vector2.Zero;
            this.ActiveTileAccelerationLength = 0f;

            // Movement provider needs velocity in order to set correct action state during reset
            m_Position.X = p_Position.X;
            m_Position.Y = p_Position.Y;
            m_Velocity.X = p_Velocity.X;
            m_Velocity.Y = p_Velocity.Y;
            this.DoReset();
        }
        public virtual void DoReset() { }

        /// <summary>
        /// Move.
        /// </summary>
        public void Move(ref Vector2 p_Position, Vector2 p_Velocity, ref bool p_DoInteract)
        {
            if (this.CanMove)
            {
                // Store initial state
                m_Position.X = p_Position.X;
                m_Position.Y = p_Position.Y;
                m_Velocity.X = p_Velocity.X;
                m_Velocity.Y = p_Velocity.Y;

                this.DoMove(ref p_DoInteract);

                // Extract final position
                p_Position = m_Position;
            }
        }
        public bool CanMove { get { return this.Conditions.IsTrue(this.Room.MainCharacter.Store); } }

        protected virtual void DoMove(ref bool p_DoInteract)
        {
            // Calculate velocity
            this.SetCandidateVelocity();
            if (this.RoomObject.IsAlive && this.RoomObject.CanAccelerateFromTile)
            {
                this.ApplyActiveTileAcceleration();
            }
            this.FinalizeCandidateVelocity();

            // Do the move
            this.MoveHorizontal(m_Velocity.X);
            this.MoveVertical(m_Velocity.Y);
            if (this.RoomObject.IsAlive)
            {
                this.EnforceTerrains();
            }

            // Update state
            this.UpdateState(ref p_DoInteract);
        }

        protected virtual void SetCandidateVelocity() { }
        protected virtual void FinalizeCandidateVelocity() { }
        protected virtual void UpdateState(ref bool p_DoInteract) { }

        #region Movement with environmental interactions

        void ApplyActiveTileAcceleration()
        {
            m_Velocity.X = m_Velocity.X + (this.ActiveTileAcceleration.X * this.Room.ViscosityFactor * this.Room.ViscosityFactor);
            m_Velocity.Y = m_Velocity.Y + (this.ActiveTileAcceleration.Y * this.Room.ViscosityFactor * this.Room.ViscosityFactor);
        }

        void MoveVertical(float p_PositionOffset)
        {
            float candidatePosition = this.Top + p_PositionOffset;
            SurfaceDirections tileCollisionSurfaceDirection = SurfaceDirections.None;
            SurfaceDirections obstacleCollisionSurfaceDirection = SurfaceDirections.None;

            if (this.RoomObject.IsAlive)
            {
                // Get tile grids data
                TileGrid activeTileGrid = this.Room.ActiveTileGrid;
                int gridXLeftIndex;
                int gridXRightIndex;
                activeTileGrid.GetGridXRange(this.Left, this.Right, out gridXLeftIndex, out gridXRightIndex);

                // Move up
                if (p_PositionOffset < 0f)
                {
                    // Process tiles
                    int gridYStartIndex = activeTileGrid.GetGridYIndex(this.Top, false);
                    int gridYEndIndex = activeTileGrid.GetGridYIndex(candidatePosition, false);
                    for (int y = gridYStartIndex - 1; y >= gridYEndIndex && tileCollisionSurfaceDirection == SurfaceDirections.None; y--)
                    {
                        // At each vertical grid point, check for a collision
                        for (int x = gridXLeftIndex; x <= gridXRightIndex; x++)
                        {
                            Tile tile = activeTileGrid.GetTileByGridPosition(x, y);
                            // If surface is deadly, pass through it for the death
                            if (tile != null && tile.BottomSide.IsSolid && (this.IsStopOnDeadlyCollision || tile.BottomSide.Offense <= this.RoomObject.Defense))
                            {
                                // Collided - position against the tile
                                candidatePosition = activeTileGrid.Top + (y + 1) * activeTileGrid.TileSize.Height;
                                tileCollisionSurfaceDirection = SurfaceDirections.Top;
                            }
                        }
                    }
                }
                // Move down
                else if (p_PositionOffset > 0f)
                {
                    // Process tiles
                    int gridYStartIndex = activeTileGrid.GetGridYIndex(this.Bottom, true);
                    int gridYEndIndex = activeTileGrid.GetGridYIndex(candidatePosition + this.Height, true);
                    for (int y = gridYStartIndex + 1; y <= gridYEndIndex && tileCollisionSurfaceDirection == SurfaceDirections.None; y++)
                    {
                        // At each vertical grid point, check for a collision
                        for (int x = gridXLeftIndex; x <= gridXRightIndex; x++)
                        {
                            Tile tile = activeTileGrid.GetTileByGridPosition(x, y);
                            // If surface is deadly, pass through it for the death
                            if (tile != null && tile.TopSide.IsSolid && (this.IsStopOnDeadlyCollision || tile.TopSide.Offense <= this.RoomObject.Defense))
                            {
                                // Collided - position against the tile
                                candidatePosition = activeTileGrid.Top + (y * activeTileGrid.TileSize.Height) - this.Height;
                                tileCollisionSurfaceDirection = SurfaceDirections.Bottom;
                            }
                        }
                    }
                }

                // Process obstacles
                RoomObjectList obstacles = this.Room.Obstacles;
                foreach (RoomObject obstacle in this.Room.Obstacles)
                {
                    this.MoveVerticalEvaluateAndHandleObstacleCollision(
                        obstacle as Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection
                        );
                }
            }

            // Pre-commit
            this.PreCommitMoveY(candidatePosition - m_Position.Y);

            // Commit
            m_Position = new Vector2(this.Left, candidatePosition);
            if (obstacleCollisionSurfaceDirection == SurfaceDirections.None && tileCollisionSurfaceDirection != SurfaceDirections.None)
            {
                this.HandleTileCollision(tileCollisionSurfaceDirection);
            }
        }

        void MoveVerticalEvaluateAndHandleObstacleCollision(
            Obstacle p_Obstacle,
            ref float candidatePosition,
            ref SurfaceDirections obstacleCollisionSurfaceDirection
            )
        {
            // Process obstacle
            if (this.EvaluateObstacleVerticalCollision(p_Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection))
            {
                this.HandleObstacleCollision(p_Obstacle, obstacleCollisionSurfaceDirection);
            }

            // Process child obstacles
            foreach (SceneObject sceneObject in p_Obstacle.SceneObjects)
            {
                this.MoveVerticalEvaluateAndHandleObstacleCollision(
                    sceneObject as Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection
                    );
            }
        }

        bool EvaluateObstacleVerticalCollision(Obstacle p_Obstacle, ref float p_CandidatePosition, ref SurfaceDirections p_CollisionSurfaceDirection)
        {
            bool isCollision = false;
            if (p_Obstacle.IsAlive)
            {
                if (p_Obstacle.BottomSide.IsSolid && p_Obstacle.BottomSide.Offense <= this.RoomObject.Defense)
                {
                    bool isObstacleCollision =
                        this.Left < p_Obstacle.Right && this.Right > p_Obstacle.Left &&
                        p_CandidatePosition < p_Obstacle.Bottom && p_CandidatePosition + this.Height > p_Obstacle.Top;
                    if (isObstacleCollision && p_Obstacle.PreviousBottom <= this.Top)
                    {
                        if (p_Obstacle.Bottom > p_CandidatePosition)
                        {
                            p_CandidatePosition = p_Obstacle.Bottom;
                            p_CollisionSurfaceDirection = SurfaceDirections.Top;
                            isCollision = true;
                        }
                    }
                }
                if (p_Obstacle.TopSide.IsSolid && p_Obstacle.TopSide.Offense <= this.RoomObject.Defense)
                {
                    bool isObstacleCollision =
                        this.Left < p_Obstacle.Right && this.Right > p_Obstacle.Left &&
                        p_CandidatePosition < p_Obstacle.Bottom && p_CandidatePosition + this.Height > p_Obstacle.Top;
                    if (isObstacleCollision && p_Obstacle.PreviousTop >= this.Bottom)
                    {
                        if (p_Obstacle.Top - this.Height < p_CandidatePosition)
                        {
                            p_CandidatePosition = p_Obstacle.Top - this.Height;
                            p_CollisionSurfaceDirection = SurfaceDirections.Bottom;
                            isCollision = true;
                        }
                    }
                }
            }
            return isCollision;
        }

        void MoveHorizontal(float p_PositionOffset)
        {
            float candidatePosition = this.Left + p_PositionOffset;
            SurfaceDirections tileCollisionSurfaceDirection = SurfaceDirections.None;
            SurfaceDirections obstacleCollisionSurfaceDirection = SurfaceDirections.None;

            if (this.RoomObject.IsAlive)
            {
                // Get tile grids data
                TileGrid activeTileGrid = this.Room.ActiveTileGrid;
                int gridYTopIndex;
                int gridYBottomIndex;
                activeTileGrid.GetGridYRange(this.Top, this.Bottom, out gridYTopIndex, out gridYBottomIndex);

                if (p_PositionOffset < 0f)
                {
                    // Process tiles
                    int gridXStartIndex = activeTileGrid.GetGridXIndex(this.Left, false);
                    int gridXEndIndex = activeTileGrid.GetGridXIndex(candidatePosition, false);
                    for (int x = gridXStartIndex - 1; x >= gridXEndIndex && tileCollisionSurfaceDirection == SurfaceDirections.None; x--)
                    {
                        // At each horizontal grid point, check for a collision
                        for (int y = gridYTopIndex; y <= gridYBottomIndex; y++)
                        {
                            Tile tile = activeTileGrid.GetTileByGridPosition(x, y);
                            // If surface is deadly, pass through it for the death
                            if (tile != null && tile.RightSide.IsSolid && (this.IsStopOnDeadlyCollision || tile.RightSide.Offense <= this.RoomObject.Defense))
                            {
                                // Collided - position against the tile
                                candidatePosition = activeTileGrid.Left + ((x + 1) * activeTileGrid.TileSize.Width);
                                tileCollisionSurfaceDirection = SurfaceDirections.Left;
                            }
                        }
                    }
                }
                else if (p_PositionOffset > 0f)
                {
                    // Process tiles
                    int gridXStartIndex = activeTileGrid.GetGridXIndex(this.Right, true);
                    int gridXEndIndex = activeTileGrid.GetGridXIndex(candidatePosition + this.Width, true);
                    for (int x = gridXStartIndex + 1; x <= gridXEndIndex && tileCollisionSurfaceDirection == SurfaceDirections.None; x++)
                    {
                        // At each horizontal grid point, check for a collision
                        for (int y = gridYTopIndex; y <= gridYBottomIndex; y++)
                        {
                            Tile tile = activeTileGrid.GetTileByGridPosition(x, y);
                            // If surface is deadly, pass through it for the death
                            if (tile != null && tile.LeftSide.IsSolid && (this.IsStopOnDeadlyCollision || tile.LeftSide.Offense <= this.RoomObject.Defense))
                            {
                                // Collided - position against the tile
                                candidatePosition = activeTileGrid.Left + (x * activeTileGrid.TileSize.Width) - this.Width;
                                tileCollisionSurfaceDirection = SurfaceDirections.Right;
                            }
                        }
                    }
                }

                // Process obstacles
                RoomObjectList obstacles = this.Room.Obstacles;
                foreach (RoomObject obstacle in this.Room.Obstacles)
                {
                    this.MoveHorizontalEvaluateAndHandleObstacleCollision(
                        obstacle as Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection
                        );
                }
            }

            // Pre-commit
            this.PreCommitMoveX(candidatePosition - m_Position.X);

            // Commit
            m_Position = new Vector2(candidatePosition, this.Top);
            if (obstacleCollisionSurfaceDirection == SurfaceDirections.None && tileCollisionSurfaceDirection != SurfaceDirections.None)
            {
                this.HandleTileCollision(tileCollisionSurfaceDirection);
            }
        }

        void MoveHorizontalEvaluateAndHandleObstacleCollision(
            Obstacle p_Obstacle,
            ref float candidatePosition,
            ref SurfaceDirections obstacleCollisionSurfaceDirection
            )
        {
            // Process obstacle
            if (this.EvaluateObstacleHorizontalCollision(p_Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection))
            {
                this.HandleObstacleCollision(p_Obstacle, obstacleCollisionSurfaceDirection);
            }

            // Process child obstacles
            foreach (SceneObject sceneObject in p_Obstacle.SceneObjects)
            {
                this.MoveHorizontalEvaluateAndHandleObstacleCollision(
                    sceneObject as Obstacle, ref candidatePosition, ref obstacleCollisionSurfaceDirection
                    );
            }
        }

        bool EvaluateObstacleHorizontalCollision(Obstacle p_Obstacle, ref float p_CandidatePosition, ref SurfaceDirections p_CollisionSurfaceDirection)
        {
            bool isCollision = false;
            if (p_Obstacle.IsAlive)
            {
                if (p_Obstacle.RightSide.IsSolid && p_Obstacle.RightSide.Offense <= this.RoomObject.Defense)
                {
                    bool isObstacleCollision =
                        p_CandidatePosition < p_Obstacle.Right && p_CandidatePosition + this.Width > p_Obstacle.Left &&
                        this.Top < p_Obstacle.Bottom && this.Bottom > p_Obstacle.Top;
                    if (isObstacleCollision && (p_Obstacle.PreviousRight <= this.Left || p_Obstacle.Right <= this.Left))
                    {
                        if (p_Obstacle.Right > p_CandidatePosition)
                        {
                            p_CandidatePosition = p_Obstacle.Right;
                            p_CollisionSurfaceDirection = SurfaceDirections.Left;
                            isCollision = true;
                        }
                    }
                }
                if (p_Obstacle.LeftSide.IsSolid && p_Obstacle.LeftSide.Offense <= this.RoomObject.Defense)
                {
                    bool isObstacleCollision =
                        p_CandidatePosition < p_Obstacle.Right && p_CandidatePosition + this.Width > p_Obstacle.Left &&
                        this.Top < p_Obstacle.Bottom && this.Bottom > p_Obstacle.Top;
                    if (isObstacleCollision && (p_Obstacle.PreviousLeft >= this.Right || p_Obstacle.Left >= this.Right))
                    {
                        if (p_Obstacle.Left - this.Width < p_CandidatePosition)
                        {
                            p_CandidatePosition = p_Obstacle.Left - this.Width;
                            p_CollisionSurfaceDirection = SurfaceDirections.Right;
                            isCollision = true;
                        }
                    }
                }
            }
            return isCollision;
        }

        /// <summary>
        /// Enforce terrains.
        /// </summary>
        void EnforceTerrains()
        {
            this.CollidedTerrain = null;
            for (int terrainIndex = 0; terrainIndex < this.Terrains.Count; terrainIndex++)
            {
                Terrain terrain = this.Terrains[terrainIndex];
                if (terrain.Constrain(this.RoomObject.Size, ref m_Position))
                {
                    this.HandleTerrainCollision(terrain);
                }
            }
        }

        /// <summary>
        /// Evaluate surroundings for solidity, friction, velocity, etc.
        /// </summary>
        static Vector2 s_TempVector;
        protected void EvaluateSurroundings()
        {
            // Clear existing surrounding data
            this.LeftSide.Reset();
            this.RightSide.Reset();
            this.TopSide.Reset();
            this.BottomSide.Reset();
            this.IsGroundInFrontOfBaseSolid = false;

            // Initialize active tile data
            this.ActiveTileAcceleration = Vector2.Zero;
            this.ActiveTileAccelerationLength = 0f;

            // Retrieve useful data
            TileGrid activeTileGrid = this.ActiveTileGrid;
            Size tileSize = activeTileGrid.TileSize;

            // Get custom point
            float groundInFrontOfBaseX = this.GetGroundInFrontOfBaseX();

            // Evaluate grid squares
            int gridXIndexStart, gridXIndexEnd;
            int gridYIndexStart, gridYIndexEnd;
            activeTileGrid.GetGridXRange(
                Math.Min(this.Left, groundInFrontOfBaseX),
                Math.Max(this.Right, groundInFrontOfBaseX),
                out gridXIndexStart, out gridXIndexEnd, true
                );
            activeTileGrid.GetGridYRange(this.Top, this.Bottom, out gridYIndexStart, out gridYIndexEnd, true);

            bool isDie = false;
            for (int gridXIndex = gridXIndexStart; !isDie && gridXIndex <= gridXIndexEnd; gridXIndex++)
            {
                float tileLeft = gridXIndex * tileSize.Width;
                float tileRight = tileLeft + tileSize.Width;
                for (int gridYIndex = gridYIndexStart; !isDie && gridYIndex <= gridYIndexEnd; gridYIndex++)
                {
                    Tile tile = activeTileGrid.GetTileByGridPosition(gridXIndex, gridYIndex);
                    if (tile != null)
                    {
                        float tileTop = gridYIndex * tileSize.Height;
                        float tileBottom = tileTop + tileSize.Height;

                        bool isDeadly = tile.Offense > this.RoomObject.Defense || (tile.IsLeftSolid && tile.IsTopSolid && tile.IsRightSolid && tile.IsBottomSolid);
                        s_TempVector.X = tile.Acceleration.X;
                        s_TempVector.Y = tile.CompensateForGravityChanges ? (tile.Acceleration.Y - this.GravityShift) : tile.Acceleration.Y;
                        this.EvaluateObstacleInteraction(
                            isDeadly, s_TempVector, Vector2.Zero,
                            tile.LeftSide, tile.TopSide, tile.RightSide, tile.BottomSide,
                            tileLeft, tileTop, tileRight, tileBottom,
                            groundInFrontOfBaseX,
                            ref isDie
                            );
                    }
                }
            }

            // Evaluate obstacles
            // REVISIT: This should be a recursive algorithm
            for (int i = 0; !isDie && i < this.Room.Obstacles.Count; i++)
            {
                // Evaluate obstacle
                Obstacle obstacle = this.Room.Obstacles[i] as Obstacle;
                if (obstacle.IsAlive && this.RoomObject != obstacle && this.RoomObject != obstacle.Parent)
                {
                    Obstacle thisObstacle = this.RoomObject as Obstacle;
                    if (thisObstacle == null || thisObstacle.Parent != obstacle)
                    {
                        // If obstacle is solid, then being squashed into the obstacle should kill the player
                        bool isDeadly = obstacle.Offense > this.RoomObject.Defense || (obstacle.IsLeftSolid && obstacle.IsTopSolid && obstacle.IsRightSolid && obstacle.IsBottomSolid);
                        this.EvaluateObstacleInteraction(
                            isDeadly, Vector2.Zero, obstacle.Velocity,
                            obstacle.LeftSide, obstacle.TopSide, obstacle.RightSide, obstacle.BottomSide,
                            obstacle.Left, obstacle.Top, obstacle.Right, obstacle.Bottom,
                            groundInFrontOfBaseX,
                            ref isDie
                            );
                    }
                }

                // Evaluate projectiles
                for (int j = 0; j < obstacle.SceneObjects.Count; j++)
                {
                    Obstacle projectile = obstacle.SceneObjects[j] as Obstacle;
                    if (projectile.IsAlive && this.RoomObject != projectile && this.RoomObject != projectile.Parent)
                    {
                        Obstacle thisObstacle = this.RoomObject as Obstacle;
                        if (thisObstacle == null || thisObstacle.Parent != obstacle)
                        {
                            // If projectile is solid, then being squashed into the obstacle should kill the player
                            bool isDeadly = projectile.Offense > this.RoomObject.Defense || (projectile.IsLeftSolid && projectile.IsTopSolid && projectile.IsRightSolid && projectile.IsBottomSolid);
                            this.EvaluateObstacleInteraction(
                                isDeadly, Vector2.Zero, projectile.Velocity,
                                projectile.LeftSide, projectile.TopSide, projectile.RightSide, projectile.BottomSide,
                                projectile.Left, projectile.Top, projectile.Right, projectile.Bottom,
                                groundInFrontOfBaseX,
                                ref isDie
                                );
                        }
                    }
                }
            }

            // Evaluate main character
            if (this.MainCharacter.IsAlive && this.RoomObject != this.MainCharacter)
            {
                bool isDeadly = this.MainCharacter.Offense > this.RoomObject.Defense;
                this.EvaluateObstacleInteraction(
                    isDeadly, Vector2.Zero, this.MainCharacter.Velocity,
                    null, null, null, null,
                    this.MainCharacter.Left, this.MainCharacter.Top, this.MainCharacter.Right, this.MainCharacter.Bottom,
                    groundInFrontOfBaseX,
                    ref isDie
                    );
            }

            // Check whether the object is touching a terrain
            if (this.CollidedTerrain != null)
            {
                if (this.CollidedTerrain.TerrainPosition == TerrainPosition.Top)
                {
                    UpdateSide(this.TopSide, true, this.CollidedTerrain.Friction, 0f);
                }
                else if (this.CollidedTerrain.TerrainPosition == TerrainPosition.Bottom)
                {
                    UpdateSide(this.BottomSide, true, this.CollidedTerrain.Friction, 0f);
                }
            }

            // Handle death
            if (isDie)
            {
                this.RoomObject.StartDeath();
            }
        }

        void EvaluateObstacleInteraction(
            bool p_ObstacleIsDeadly, Vector2 p_ObstacleAcceleration, Vector2 p_ObstacleVelocity,
            SurfaceInformation p_ObstacleLeftSide, SurfaceInformation p_ObstacleTopSide,
            SurfaceInformation p_ObstacleRightSide, SurfaceInformation p_ObstacleBottomSide,
            float p_ObstacleLeft, float p_ObstacleTop,
            float p_ObstacleRight, float p_ObstacleBottom,
            float p_GroundInFrontOfBaseX, ref bool isDie
            )
        {
            bool isOverlapLeft = (this.Left < p_ObstacleRight);
            bool isOverlapRight = (this.Right > p_ObstacleLeft);
            bool isOverlapHorizontal = isOverlapLeft && isOverlapRight;
            bool isOverlapTop = (this.Top < p_ObstacleBottom);
            bool isOverlapBottom = (this.Bottom > p_ObstacleTop);
            bool isOverlapVertical = isOverlapTop && isOverlapBottom;

            bool isBorderLeft = (this.Left == p_ObstacleRight);
            bool isBorderRight = (this.Right == p_ObstacleLeft);
            bool isBorderTop = (this.Top == p_ObstacleBottom);
            bool isBorderBottom = (this.Bottom == p_ObstacleTop);

            // Handle overlapping tile
            if (isOverlapHorizontal && isOverlapVertical)
            {
                // If room object overlaps with deadly tile, kill them
                if (p_ObstacleIsDeadly)
                {
                    isDie = true;
                }
                else
                {
                    // Record tile with the strongest acceleration
                    float length = p_ObstacleAcceleration.Length();
                    if (length > this.ActiveTileAccelerationLength)
                    {
                        this.ActiveTileAcceleration = p_ObstacleAcceleration;
                        this.ActiveTileAccelerationLength = length;
                    }
                }
            }
            else
            {
                // Handle neighbouring tile
                if (isOverlapVertical)
                {
                    if (isBorderLeft && p_ObstacleRightSide != null && p_ObstacleRightSide.IsSolid)
                    {
                        UpdateSide(this.LeftSide, p_ObstacleRightSide, p_ObstacleVelocity.Y);
                    }
                    else if (isBorderRight && p_ObstacleLeftSide != null && p_ObstacleLeftSide.IsSolid)
                    {
                        UpdateSide(this.RightSide, p_ObstacleLeftSide, p_ObstacleVelocity.Y);
                    }
                }
                else if (isOverlapHorizontal)
                {
                    if (isBorderTop && p_ObstacleBottomSide != null && p_ObstacleBottomSide.IsSolid)
                    {
                        UpdateSide(this.TopSide, p_ObstacleBottomSide, p_ObstacleVelocity.X);
                    }
                    else if (isBorderBottom && p_ObstacleTopSide != null && p_ObstacleTopSide.IsSolid)
                    {
                        UpdateSide(this.BottomSide, p_ObstacleTopSide, p_ObstacleVelocity.X);
                    }
                }

                // Look at ground in front of feet
                if (isBorderBottom && p_ObstacleTopSide != null && p_ObstacleTopSide.IsSolid && p_GroundInFrontOfBaseX >= p_ObstacleLeft && p_GroundInFrontOfBaseX <= p_ObstacleRight)
                {
                    this.IsGroundInFrontOfBaseSolid = true;
                }
            }
        }

        static void UpdateSide(SurfaceInformation p_Side, SurfaceInformation p_SurfaceInfo, float p_ObstacleVelocity)
        {
            UpdateSide(p_Side, p_SurfaceInfo.IsSolid, p_SurfaceInfo.Friction, p_ObstacleVelocity + p_SurfaceInfo.Velocity);
        }

        static void UpdateSide(SurfaceInformation p_Side, bool p_ObstacleIsSolid, float p_ObstacleFriction, float p_ObstacleVelocity)
        {
            bool isSolid = p_Side.IsSolid || p_ObstacleIsSolid;
            float friction = Math.Max(p_Side.Friction, p_ObstacleFriction);
            float velocity = (Math.Abs(p_ObstacleVelocity) > Math.Abs(p_Side.Velocity)) ? p_ObstacleVelocity : p_Side.Velocity;
            p_Side.Update(isSolid, friction, velocity);
        }

        // By default, keep moving on collision, with the assumption that the movement provider
        // will detect that the room object is within another solid object and die while maintaining
        // its horizontal velocity from before the collision
        protected virtual bool IsStopOnDeadlyCollision { get { return false; } }

        protected virtual void PreCommitMoveX(float p_Offset) { }
        protected virtual void PreCommitMoveY(float p_Offset) { }
        protected virtual void HandleObstacleCollision(Obstacle p_Obstacle, SurfaceDirections p_SurfaceDirection) { }
        protected virtual void HandleTileCollision(SurfaceDirections p_CollisionType) { }
        protected virtual void HandleTerrainCollision(Terrain p_Terrain) { }
        protected virtual float GetGroundInFrontOfBaseX() { return this.MidX; }

        #endregion
    }

    /// <summary>
    /// Xml room object movement provider reader.
    /// </summary>
    public abstract class XmlRoomObjectMovementProviderReader
    {
        public abstract RoomObjectMovementProvider CreateInstance(OSATypes.TemplateMovementProvider movementProvider, OSATypes.TemplateMovementProvider p_TemplateRoomObject);
    }

    /// <summary>
    /// String to xml room object movement provider p_Reader dictionary class.
    /// </summary>
    public class StringToXmlRoomObjectMovementProviderReaderDictionary : Dictionary<string, XmlRoomObjectMovementProviderReader> { }
}
