using System;
using System.Collections.ObjectModel;
using LiveSplit.ComponentUtil;
using LiveSplit.dotStart.Common.Splitter;

namespace LiveSplit.dotStart.PetThePup {
  /// <summary>
  /// Provides access to game state data and events for the Pet the Pup at the Party game.
  /// </summary>
  public class GameMemoryImpl : SharableGameMemory {
    
    public uint PupCount { get; private set; }
    public uint ConversationCount { get; private set; }

    /// <summary>
    /// Grants access to an instance of this type.
    /// 
    /// This is used in order to allow sharing of the type between components (such as the UI and
    /// auto splitter component).
    /// </summary>
    public static GameMemoryImpl Instance {
      get {
        GameMemoryImpl instance;

        if (!_instance.TryGetTarget(out instance)) {
          instance = new GameMemoryImpl();
          _instance.SetTarget(instance);
        }

        instance.Claim();
        return instance;
      }
    }

    public event EventHandler OnGameStart;
    public event EventHandler OnGameReset;
    public event EventHandler OnGameAdvance;
    
    private static readonly DeepPointer LevelPointer = new DeepPointer(0x10A5500);
    private static readonly DeepPointer PuppyCounterPointer =
      new DeepPointer("mono.dll", 0x001F8CC0, 0xF0, 0x44, 0x58C);
    private static readonly DeepPointer PeopleCounterPointer =
      new DeepPointer("PetThePup.exe", 0x01021290, 0x4E4, 0x3A8, 0x0, 0x364);

    /// <inheritdoc />
    protected override ReadOnlyCollection<string> ProcessNames => Array.AsReadOnly(new[] {
      "PetThePup"
    });

    /// <summary>
    /// Indicates whether the player has actually entered a game (e.g. has pressed the new game
    /// button).
    /// </summary>
    private bool _inGame;

    /// <summary>
    /// Stores the amount of pups as indicated in memory within the last iteration.
    /// </summary>
    private uint _pupCounter;

    /// <summary>
    /// Stores a weak reference to the game memory instance.
    /// </summary>
    private static readonly WeakReference<GameMemoryImpl> _instance = new WeakReference<GameMemoryImpl>(null);

    private GameMemoryImpl() {
    }

    /// <inheritdoc />
    protected override void ResetValues() {
      base.ResetValues();

      this._inGame = false;
      this._pupCounter = 0;
      
      this.ParentThread.Post(d => {
        this.PupCount = 0;
        this.ConversationCount = 0;
      }, null);
    }

    /// <inheritdoc />
    protected override void UpdateValues() {
      uint level = LevelPointer.Deref(this.Process, (uint) 0);
      uint puppyCounter = PuppyCounterPointer.Deref(this.Process, (uint) 0);
      uint peopleCounter = PeopleCounterPointer.Deref(this.Process, (uint) 0);

      if (!this._inGame) {
        // if we detect any of the menu level values or if the puppy counter is not set to zero,
        // we'll skip this execution and try again at a later time
        if (level == 0 || level == 101 || puppyCounter != 0) {
          return;
        }

        // otherwise, we'll update the internal state to reflect the new in-game status and notify
        // all watchers of this change
        this._inGame = true;
        this.ParentThread.Post(d => this.OnGameStart?.Invoke(this, EventArgs.Empty), null);
      } else {
        // check whether the game has been reset and if so, update our local state to reflect this
        // change
        if (level == 0 || level == 101 || puppyCounter < this._pupCounter) {
          this.ResetValues();

          this.ParentThread.Post(d => this.OnGameReset?.Invoke(this, EventArgs.Empty), null);
        } else if (puppyCounter > this._pupCounter) {
          this.ParentThread.Post(d => this.OnGameAdvance?.Invoke(this, EventArgs.Empty), null);
        }
      }

      this._pupCounter = puppyCounter;
      
      this.ParentThread.Post(d => {
        this.PupCount = puppyCounter;
        this.ConversationCount = peopleCounter;
      }, null);
    }
  }
}
