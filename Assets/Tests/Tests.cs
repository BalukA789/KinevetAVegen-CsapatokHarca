using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class LudoPlayModeTests
{
    private GameManager gameManager;
    private LudoBoard board;

    [SetUp]
    public void Setup()
    {
        // Betöltjük a scene-t, ami a játékot tartalmazza
        gameManager = Object.FindObjectOfType<GameManager>();
        board = Object.FindObjectOfType<LudoBoard>();

        Assert.IsNotNull(gameManager, "GameManager nincs a scene-ben!");
        Assert.IsNotNull(board, "LudoBoard nincs a scene-ben!");
    }

    // 1️⃣ Teszt: Spawn-ról 6-tal ki tud lépni a figura
    [UnityTest]
    public IEnumerator SpawnRollSixMovesFigure()
    {
        // Válassz egy kék figurát, ami a spawn-on van
        var blueFigure = Object.FindObjectsOfType<Figure>()[0];
        blueFigure.teamColor = Figure.TeamColor.Blue;
        blueFigure.currentField = board.mainTrack[0].GetComponent<FieldMarker>();

        gameManager.currentTurn = Figure.TeamColor.Blue;
        gameManager.lastRoll = 6;

        gameManager.HandlePlayerMove(blueFigure);

        yield return null; // Várunk egy frame-et

        // Ellenőrizzük, hogy a figura elmozdult a spawn-ról
        Assert.AreNotEqual(board.mainTrack[0].position, blueFigure.transform.position, "A figura nem lépett ki a spawn-ról!");
    }

    // 2️⃣ Teszt: Spawn-ról nem tud lépni, ha nem 6-ot dob
    [UnityTest]
    public IEnumerator FigureCannotMoveWhenNoDiceSix()
    {
        var figure = Object.FindObjectsOfType<Figure>()[0];
        figure.teamColor = Figure.TeamColor.Blue;
        figure.currentField = board.mainTrack[0].GetComponent<FieldMarker>();

        gameManager.currentTurn = Figure.TeamColor.Blue;
        gameManager.lastRoll = 3;

        bool canMove = gameManager.CanFigureMove(figure, gameManager.lastRoll);
        yield return null;

        Assert.IsFalse(canMove, "A figura nem tudna lépni, mert spawn-ról csak 6-tal lehet kimenni.");
    }

    // 3️⃣ Teszt: Két figura finish-re érkezése után győztes szín beállítása
    [UnityTest]
    public IEnumerator TwoFiguresFinishDeclaresWinner()
    {
        // Példa: kék csapatból két figura a finish-re
        var finish = board.blueFinish.GetComponent<FieldMarker>();
        var figures = Object.FindObjectsOfType<Figure>();
        int count = 0;
        foreach (var f in figures)
        {
            if (f.teamColor == Figure.TeamColor.Blue)
            {
                f.currentField = finish;
                count++;
                if (count == 2) break;
            }
        }

        // Feltételezzük, hogy van GameManager-ben CheckForWinner() vagy a MoveToField ellenőrzi
        gameManager.CheckWinner(Figure.TeamColor.Blue); // Hozz létre vagy implementáld a metódust
        yield return null;

        Assert.AreEqual(Figure.TeamColor.Blue, board.winner, "A kék csapatnak kellene nyernie!");
    }
}
