using System;
using System.Collections.Generic;
using System.Linq;
using SnakeGameBackend.Entities;
using SnakeGameBackend.Interfaces;

namespace SnakeGameBackend.Services
{
    public class GameService
    {
        private readonly GameStateService _gameState;
        private readonly CollisorService _collisorService;
        public GameState State => _gameState.State;

        public GameService(
            GameStateService gameStateService, 
            CollisorService collisorService
        )
        {
            _gameState = gameStateService;
            _collisorService = collisorService;
            _collisorService.OnCollision += OnCollision;
        }

        public void Update()
        {
            _gameState.State.Snakes.ForEach(snake => snake.Update());

            var collidables = new List<ICollidable>();

            if (!_gameState.State.Snakes.Any()) return;

            collidables.AddRange(_gameState.State.Snakes);
            collidables.AddRange(_gameState.State.Fruits);

            _collisorService.setCollidables(collidables).Check();
        }

        public void OnCollision(object collisorService, CollisionEventArgs args)
        {
            var fruit = args.Collidable1.GetType() == typeof(Fruit) ? args.Collidable1 : args.Collidable2;
            
            if (fruit.GetType() != typeof(Fruit)) return;

            _gameState.RemoveFruit(fruit.Id);
            _gameState.GenerateFruit();            
        }
    }
}