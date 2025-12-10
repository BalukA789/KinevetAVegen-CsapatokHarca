using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class LudoPlayModeTests
{
    private GameManager gameManager;
    private LudoBoard board;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // GameManager
        var gmObj = new GameObject("GameManager_TEST");
        gameManager = gmObj.AddComponent<GameManager>();

        // Board
        var boardObj = new GameObject("Board_TEST");
        board = boardObj.AddComponent<LudoBoard>();

        // Track
        board.mainTrack = new Transform[52];
        for (int i = 0; i < 52; i++)
        {
            var f = new GameObject("Track_" + i);
            var fm = f.AddComponent<FieldMarker>();
            fm.type = FieldMarker.FieldType.Normal;
            board.mainTrack[i] = f.transform;
        }

        // Blue spawn
        board.blueBase = new Transform[4];
        var spawnObj = new GameObject("Spawn");
        var spawnFM = spawnObj.AddComponent<FieldMarker>();
        spawnFM.type = FieldMarker.FieldType.Spawn;
        board.blueBase[0] = spawnObj.transform;

        // Finish
        var finishObj = new GameObject("Finish");
        var finishFM = finishObj.AddComponent<FieldMarker>();
        finishFM.type = FieldMarker.FieldType.Finish;
        board.blueFinish = finishObj.transform;

        yield return null;
    }

   

    // 1. Spawn-ról nem léphet, ha nem 6
    [UnityTest]
    public IEnumerator FigureCannotMoveWhenRollNotSix()
    {
        var figObj = new GameObject("Fig2");
        var fig = figObj.AddComponent<Figure>();
        fig.board = board;
        fig.teamColor = Figure.TeamColor.Blue;

        var spawnField = board.blueBase[0].GetComponent<FieldMarker>();
        spawnField.type = FieldMarker.FieldType.Spawn;
        fig.currentField = spawnField;

        // Ellenőrizzük a CanFigureMove-ot: spawn-ról csak 6-tal lehet kimenni
        bool canMove = gameManager.CanFigureMove(fig, 3);

        yield return null;

        Assert.IsFalse(canMove, "Spawn-ról nem kellene tudni lépni, ha nem 6-ot dobtak.");
    }

    // 2. Győztes teszt
    [UnityTest]
    public IEnumerator TwoFiguresFinishDeclaresWinner()
    {
        var finish = board.blueFinish.GetComponent<FieldMarker>();

        var f1 = new GameObject("F1").AddComponent<Figure>();
        var f2 = new GameObject("F2").AddComponent<Figure>();

        f1.teamColor = Figure.TeamColor.Blue;
        f2.teamColor = Figure.TeamColor.Blue;

        f1.board = board;
        f2.board = board;

        finish.AddFigure(f1);
        finish.AddFigure(f2);

        board.DeclareWinner(Figure.TeamColor.Blue);

        yield return null;

        Assert.AreEqual(Figure.TeamColor.Blue, board.winner);
        Assert.IsTrue(board.gameOver);
    }
    // 3. 4️⃣ Ha a játék véget ért, a figura nem tud mozogni
    [UnityTest]
    public IEnumerator FigureDoesNotMoveWhenGameOver()
    {
        var figObj = new GameObject("Fig_GameOver");
        var figure = figObj.AddComponent<Figure>();
        figure.board = board;
        figure.teamColor = Figure.TeamColor.Blue;

        // Mezők
        var fromObj = new GameObject("FromTest");
        var from = fromObj.AddComponent<FieldMarker>();
        from.type = FieldMarker.FieldType.Normal;

        var toObj = new GameObject("ToTest");
        var to = toObj.AddComponent<FieldMarker>();
        to.type = FieldMarker.FieldType.Normal;

        // Kezdő pozíció
        figure.currentField = from;
        Vector3 startPos = figure.transform.position;

        // Játék vége
        board.gameOver = true;

        // Próbáljuk megmozgatni
        figure.MoveToField(to);

        yield return null;

        // Nem változhatott meg a pozíció
        Assert.AreEqual(startPos, figure.transform.position,
            "GameOver esetén a figurának nem szabadna mozognia!");
    }

}
