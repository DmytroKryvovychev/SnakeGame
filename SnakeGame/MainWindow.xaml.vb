Class MainWindow 
    Dim k As Key
    Dim timer As New System.Windows.Threading.DispatcherTimer
    Dim lenght As Integer = 5
    Dim time As Integer = 700000
    Dim speed As New TimeSpan(time)
    Dim startingPoint As New Point(96, 96)
    Dim currentposition As New Point()
    Dim bonusPoints As New List(Of Point)
    Dim snakepoints As New List(Of Point)
    Dim Rmd As New Random()
    Dim score As Integer = 0
    Dim direction As Integer = 0
    Dim previousDirection As Integer = 0
    Dim itsover As Boolean = False
    Dim headSize As Integer = 8
    Dim settings As New MySettings
    Dim vicScore As Integer = 0
    Dim vicName As String
    

    Enum moving
        Up = 1
        Down = 2
        Left = 3
        Right = 4
    End Enum
    Public Sub New()


        InitializeComponent()

        timer.Interval = speed
        timer.Start()
        AddHandler timer.Tick, AddressOf timer_Tick
        AddHandler Me.KeyDown, AddressOf MainWindow_KeyDown
        AddHandler Me.KeyUp, AddressOf MainWindow_KeyUp
        direction = 0
        snakepoints.Clear()
        paintSnake(startingPoint)
        currentposition = startingPoint
        For n As Integer = 0 To 1
            paintBonus(n)
            n += 1
        Next
        lbl.Content = "Score: 0"
        lblGameOver.Visibility = Windows.Visibility.Collapsed
        itsover = False

    End Sub
    Private Sub MainWindow_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
        If inG.Visibility = Windows.Visibility.Visible Then
            If e.Key = Key.F2 Then
                inG.Visibility = Visibility.Collapsed
                inGb.Visibility = Visibility.Collapsed
            End If
            Exit Sub
        Else
            Input.ChangeState(e.Key, True)
            If e.Key = Key.P Then
                timer.IsEnabled = False
            End If
            If timer.IsEnabled = False Then
                If e.Key = Key.O Then
                    timer.IsEnabled = True
                End If
            End If    
        End If
    End Sub
    Private Sub MainWindow_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyUp
        Input.ChangeState(e.Key, False)
        
    End Sub

    Private Sub paintSnake(ByVal currentposition As Point)
        Dim newRect As New Rectangle
        Dim count As Integer

        newRect.Fill = Brushes.Black
        newRect.Width = headSize
        newRect.Height = headSize

        Canvas.SetTop(newRect, currentposition.Y)
        Canvas.SetLeft(newRect, currentposition.X)    

        'count = paintCanvas.Children.Count
        count = snakepoints.Count
        paintCanvas.Children.Add(newRect)
        snakepoints.Add(currentposition)

        If count > lenght Then
            paintCanvas.Children.RemoveAt(count - lenght)
            snakepoints.RemoveAt(count - lenght)
        End If


    End Sub
    Private Sub Move()
        If itsover = False Then
            Select Case direction
                Case moving.Left
                    currentposition.X -= headSize
                    paintSnake(currentposition)
                    Exit Sub
                Case moving.Right
                    currentposition.X += headSize
                    paintSnake(currentposition)
                    Exit Sub
                Case moving.Up
                    currentposition.Y -= headSize
                    paintSnake(currentposition)
                    Exit Sub
                Case moving.Down
                    currentposition.Y += headSize
                    paintSnake(currentposition)
                    Exit Sub
            End Select
        Else
            Dim ne As New MainWindow
            Dim msg As New MsgBoxResult

            timer.Stop()
            timer.IsEnabled = False
            RemoveHandler timer.Tick, AddressOf timer_Tick
            lblGameOver.Visibility = Windows.Visibility.Visible
            msg = MsgBox("You lose! Your score is " + score.ToString(), vbCritical + vbOKCancel, "Game Over")
            Input.ChangeState(Key.Right, False)
            Input.ChangeState(Key.Left, False)
            Input.ChangeState(Key.Down, False)
            Input.ChangeState(Key.Up, False)
            Select Case msg
                Case MsgBoxResult.Ok
                    ne.Show()
                    Me.Close()
                    Exit Select
                Case MsgBoxResult.Cancel
                    Me.Close()
                    ne.Close()
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        'Альтернативный вариант изменения направления движения по нажатию клавиши
        If Input.Keypressed(Key.Down) And direction <> moving.Up Then
            direction = moving.Down
        ElseIf Input.Keypressed(Key.Up) And direction <> moving.Down Then
            direction = moving.Up
        ElseIf Input.Keypressed(Key.Right) And direction <> moving.Left Then
            direction = moving.Right
        ElseIf Input.Keypressed(Key.Left) And direction <> moving.Right Then
            direction = moving.Left
        End If

        If Input.Keypressed(Key.F2) Then
            inG.Visibility = Visibility.Collapsed
            inGb.Visibility = Visibility.Collapsed
        End If

            Move()

            'Назначение границ окна
        If ((currentposition.X < 0) Or (currentposition.X > 490) Or (currentposition.Y < 0) Or (currentposition.Y > 440)) Then
            GameOver()
            Exit Sub
        End If

        'Появление бонуса, начисление очков, увеличение змейки
        Dim n As Integer = 0
            Dim point As Point
            For Each point In bonusPoints
            If ((Math.Abs(point.X - currentposition.X) < headSize) And (Math.Abs(point.Y - currentposition.Y) < headSize)) Then
                lenght += 1
                score += 10
                lbl.Content = "Score: " & score
                bonusPoints.RemoveAt(n)
                paintCanvas.Children.RemoveAt(n)
                paintBonus(n)
                Exit For
            End If
                n = n + 1
        Next
        timer.Interval = New TimeSpan(-3450 * lenght + 717250)
            For q As Integer = 1 To (snakepoints.Count - 3) Step 1
                Dim point1 As New Point(snakepoints(q).X, snakepoints(q).Y)
                If ((Math.Abs(point1.X - currentposition.X) < 8) And (Math.Abs(point1.Y - currentposition.Y) < 8)) Then
                    GameOver()
                    Exit Sub
                End If
        Next

    End Sub

    Private Sub paintBonus(ByVal index As Integer)
        'Описание бонус-поинта
        Dim bonusPoint As New Point(Rmd.Next(0, 400 / 8) * 8, Rmd.Next(0, 400 / 8) * 8)
        Dim newRect As New Rectangle

        newRect.Fill = Brushes.Red
        newRect.Width = headSize
        newRect.Height = headSize

        Canvas.SetTop(newRect, bonusPoint.Y)
        Canvas.SetLeft(newRect, bonusPoint.X)
        paintCanvas.Children.Insert(index, newRect)
        bonusPoints.Insert(index, bonusPoint)


    End Sub

    Private Sub GameOver()
        Dim ne As New MainWindow
        Dim msg As New MsgBoxResult
        

        itsover = True
        timer.Stop()
        timer.IsEnabled = False
        Input.ChangeState(System.Windows.Input.Key.Right, False)
        Input.ChangeState(System.Windows.Input.Key.Left, False)
        Input.ChangeState(System.Windows.Input.Key.Down, False)
        Input.ChangeState(System.Windows.Input.Key.Up, False)
        lblGameOver.Visibility = Windows.Visibility.Visible
        If score > settings.vicScore Then
            settings.vicName = InputBox("Введите своё имя", "Новый рекорд")
            settings.vicScore = score
            settings.Save()
        End If
        msg = MsgBox("You lose! Your score is " + score.ToString() & vbCr & "      Start New Game?", vbCritical + vbOKCancel, "Game Over")
        Select Case msg
            Case MsgBoxResult.Ok
                ne.Show()
                Me.Close()
                ne.inG.Visibility = Visibility.Collapsed
                ne.inGb.Visibility = Visibility.Collapsed
                Exit Select
            Case MsgBoxResult.Cancel
                Me.Close()
                ne.Close()
                Exit Select
        End Select
    End Sub

    Private Sub Exit_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles [Exit].Click
        Me.Close()
    End Sub

    Private Sub nG_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles nG.Click
        inG.Visibility = Visibility.Collapsed
        inGb.Visibility = Visibility.Collapsed    
    End Sub

    Private Sub About_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles About.Click
        MsgBox(settings.vicName & " " & settings.vicScore, vbInformation)
    End Sub

End Class
