﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.IO
Imports EmberAPI
Imports NLog
Imports System.Diagnostics
Public Class dlgEditTVSeason

#Region "Fields"

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Private tmpDBTVSeason As New Structures.DBTV

#End Region 'Fields

#Region "Properties"

    Public ReadOnly Property Result As Structures.DBTV
        Get
            Return tmpDBTVSeason
        End Get
    End Property

#End Region 'Properties

#Region "Methods"

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        Me.Left = Master.AppPos.Left + (Master.AppPos.Width - Me.Width) \ 2
        Me.Top = Master.AppPos.Top + (Master.AppPos.Height - Me.Height) \ 2
        Me.StartPosition = FormStartPosition.Manual
    End Sub

    Public Overloads Function ShowDialog(ByVal DBTVSeason As Structures.DBTV) As DialogResult
        Me.tmpDBTVSeason = DBTVSeason
        Return MyBase.ShowDialog()
    End Function

    Private Sub btnRemoveSeasonBanner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSeasonBanner.Click
        Me.pbSeasonBanner.Image = Nothing
        Me.pbSeasonBanner.Tag = Nothing
        Me.tmpDBTVSeason.ImagesContainer.Banner = New MediaContainers.Image
        Me.tmpDBTVSeason.BannerPath = String.Empty
    End Sub

    Private Sub btnRemoveSeasonFanart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSeasonFanart.Click
        Me.pbSeasonFanart.Image = Nothing
        Me.pbSeasonFanart.Tag = Nothing
        Me.tmpDBTVSeason.ImagesContainer.Fanart = New MediaContainers.Image
        Me.tmpDBTVSeason.FanartPath = String.Empty
    End Sub

    Private Sub btnRemoveSeasonLandscape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSeasonLandscape.Click
        Me.pbSeasonLandscape.Image = Nothing
        Me.pbSeasonLandscape.Tag = Nothing
        Me.tmpDBTVSeason.ImagesContainer.Landscape = New MediaContainers.Image
        Me.tmpDBTVSeason.LandscapePath = String.Empty
    End Sub

    Private Sub btnRemoveSeasonPoster_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSeasonPoster.Click
        Me.pbSeasonPoster.Image = Nothing
        Me.pbSeasonPoster.Tag = Nothing
        Me.tmpDBTVSeason.ImagesContainer.Poster = New MediaContainers.Image
        Me.tmpDBTVSeason.PosterPath = String.Empty
    End Sub

    Private Sub btnSetSeasonBannerDL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonBannerDL.Click
        Using dImgManual As New dlgImgManual
            If dImgManual.ShowDialog() = DialogResult.OK Then
                Dim tImage As MediaContainers.Image = dImgManual.Results
                If tImage.WebImage.Image IsNot Nothing Then
                    Me.tmpDBTVSeason.ImagesContainer.Banner = tImage
                    Me.pbSeasonBanner.Image = tImage.WebImage.Image
                    Me.pbSeasonBanner.Tag = tImage

                    Me.lblSeasonBannerSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonBanner.Image.Width, Me.pbSeasonBanner.Image.Height)
                    Me.lblSeasonBannerSize.Visible = True
                End If
            End If
        End Using
    End Sub

    Private Sub btnSetSeasonBannerScrape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonBannerScrape.Click
        Dim tImage As MediaContainers.Image = ModulesManager.Instance.TVSingleImageOnly(Me.tmpDBTVSeason.TVShow.Title, Convert.ToInt32(Me.tmpDBTVSeason.ShowID), Me.tmpDBTVSeason.TVShow.TVDB, Enums.ModifierType.SeasonBanner, Me.tmpDBTVSeason.TVSeason.Season, 0, Me.tmpDBTVSeason.Language, Me.tmpDBTVSeason.Ordering, CType(Me.tmpDBTVSeason.ImagesContainer.Banner, MediaContainers.Image))

        If tImage IsNot Nothing AndAlso tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Banner = tImage
            Me.pbSeasonBanner.Image = tImage.WebImage.Image
            Me.pbSeasonBanner.Tag = tImage

            Me.lblSeasonBannerSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonBanner.Image.Width, Me.pbSeasonBanner.Image.Height)
            Me.lblSeasonBannerSize.Visible = True
        End If
    End Sub

    Private Sub btnSetSeasonBannerLocal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonBannerLocal.Click
        With ofdImage
            .InitialDirectory = Me.tmpDBTVSeason.ShowPath
            .Filter = Master.eLang.GetString(497, "Images") + "|*.jpg;*.png"
            .FilterIndex = 0
        End With

        If ofdImage.ShowDialog() = DialogResult.OK Then
            Dim tImage As New MediaContainers.Image
            tImage.WebImage.FromFile(ofdImage.FileName)
            If tImage.WebImage.Image IsNot Nothing Then
                Me.tmpDBTVSeason.ImagesContainer.Banner = tImage
                Me.pbSeasonBanner.Image = tImage.WebImage.Image
                Me.pbSeasonBanner.Tag = tImage

                Me.lblSeasonBannerSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonBanner.Image.Width, Me.pbSeasonBanner.Image.Height)
                Me.lblSeasonBannerSize.Visible = True
            End If
        End If
    End Sub

    Private Sub btnSetSeasonFanartDL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonFanartDL.Click
        Using dImgManual As New dlgImgManual
            If dImgManual.ShowDialog() = DialogResult.OK Then
                Dim tImage As MediaContainers.Image = dImgManual.Results
                If tImage.WebImage.Image IsNot Nothing Then
                    Me.tmpDBTVSeason.ImagesContainer.Fanart = tImage
                    Me.pbSeasonFanart.Image = tImage.WebImage.Image
                    Me.pbSeasonFanart.Tag = tImage

                    Me.lblSeasonFanartSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonFanart.Image.Width, Me.pbSeasonFanart.Image.Height)
                    Me.lblSeasonFanartSize.Visible = True
                End If
            End If
        End Using
    End Sub

    Private Sub btnSetSeasonFanartScrape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonFanartScrape.Click
        Dim tImage As MediaContainers.Image = ModulesManager.Instance.TVSingleImageOnly(Me.tmpDBTVSeason.TVShow.Title, Convert.ToInt32(Me.tmpDBTVSeason.ShowID), Me.tmpDBTVSeason.TVShow.TVDB, Enums.ModifierType.SeasonFanart, Me.tmpDBTVSeason.TVSeason.Season, 0, Me.tmpDBTVSeason.Language, Me.tmpDBTVSeason.Ordering, CType(Me.tmpDBTVSeason.ImagesContainer.Fanart, MediaContainers.Image))

        If tImage IsNot Nothing AndAlso tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Fanart = tImage
            Me.pbSeasonFanart.Image = tImage.WebImage.Image
            Me.pbSeasonFanart.Tag = tImage

            Me.lblSeasonFanartSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonFanart.Image.Width, Me.pbSeasonFanart.Image.Height)
            Me.lblSeasonFanartSize.Visible = True
        End If
    End Sub

    Private Sub btnSetSeasonFanartLocal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonFanartLocal.Click
        With ofdImage
            .InitialDirectory = Me.tmpDBTVSeason.ShowPath
            .Filter = Master.eLang.GetString(497, "Images") + "|*.jpg;*.png"
            .FilterIndex = 4
        End With

        If ofdImage.ShowDialog() = DialogResult.OK Then
            Dim tImage As New MediaContainers.Image
            tImage.WebImage.FromFile(ofdImage.FileName)
            If tImage.WebImage.Image IsNot Nothing Then
                Me.tmpDBTVSeason.ImagesContainer.Fanart = tImage
                Me.pbSeasonFanart.Image = tImage.WebImage.Image
                Me.pbSeasonFanart.Tag = tImage

                Me.lblSeasonFanartSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonFanart.Image.Width, Me.pbSeasonFanart.Image.Height)
                Me.lblSeasonFanartSize.Visible = True
            End If
        End If
    End Sub

    Private Sub btnSetSeasonLandscapeDL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonLandscapeDL.Click
        Using dImgManual As New dlgImgManual
            If dImgManual.ShowDialog() = DialogResult.OK Then
                Dim tImage As MediaContainers.Image = dImgManual.Results
                If tImage.WebImage.Image IsNot Nothing Then
                    Me.tmpDBTVSeason.ImagesContainer.Landscape = tImage
                    Me.pbSeasonLandscape.Image = tImage.WebImage.Image
                    Me.pbSeasonLandscape.Tag = tImage

                    Me.lblSeasonLandscapeSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonLandscape.Image.Width, Me.pbSeasonLandscape.Image.Height)
                    Me.lblSeasonLandscapeSize.Visible = True
                End If
            End If
        End Using
    End Sub

    Private Sub btnSetSeasonLandscapeScrape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonLandscapeScrape.Click
        Dim tImage As MediaContainers.Image = ModulesManager.Instance.TVSingleImageOnly(Me.tmpDBTVSeason.TVShow.Title, Convert.ToInt32(Me.tmpDBTVSeason.ShowID), Me.tmpDBTVSeason.TVShow.TVDB, Enums.ModifierType.SeasonLandscape, Me.tmpDBTVSeason.TVSeason.Season, 0, Me.tmpDBTVSeason.Language, Me.tmpDBTVSeason.Ordering, CType(Me.tmpDBTVSeason.ImagesContainer.Landscape, MediaContainers.Image))

        If tImage IsNot Nothing AndAlso tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Landscape = tImage
            Me.pbSeasonLandscape.Image = tImage.WebImage.Image
            Me.pbSeasonLandscape.Tag = tImage

            Me.lblSeasonLandscapeSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonLandscape.Image.Width, Me.pbSeasonLandscape.Image.Height)
            Me.lblSeasonLandscapeSize.Visible = True
        End If
    End Sub

    Private Sub btnSetSeasonLandscapeLocal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonLandscapeLocal.Click
        With ofdImage
            .InitialDirectory = Me.tmpDBTVSeason.ShowPath
            .Filter = Master.eLang.GetString(497, "Images") + "|*.jpg;*.png"
            .FilterIndex = 0
        End With

        If ofdImage.ShowDialog() = DialogResult.OK Then
            Dim tImage As New MediaContainers.Image
            tImage.WebImage.FromFile(ofdImage.FileName)
            If tImage.WebImage.Image IsNot Nothing Then
                Me.tmpDBTVSeason.ImagesContainer.Landscape = tImage
                Me.pbSeasonLandscape.Image = tImage.WebImage.Image
                Me.pbSeasonLandscape.Tag = tImage

                Me.lblSeasonLandscapeSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonLandscape.Image.Width, Me.pbSeasonLandscape.Image.Height)
                Me.lblSeasonLandscapeSize.Visible = True
            End If
        End If
    End Sub

    Private Sub btnSetSeasonPosterDL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonPosterDL.Click
        Using dImgManual As New dlgImgManual
            If dImgManual.ShowDialog() = DialogResult.OK Then
                Dim tImage As MediaContainers.Image = dImgManual.Results
                If tImage.WebImage.Image IsNot Nothing Then
                    Me.tmpDBTVSeason.ImagesContainer.Poster = tImage
                    Me.pbSeasonPoster.Image = tImage.WebImage.Image
                    Me.pbSeasonPoster.Tag = tImage

                    Me.lblSeasonPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonPoster.Image.Width, Me.pbSeasonPoster.Image.Height)
                    Me.lblSeasonPosterSize.Visible = True
                End If
            End If
        End Using
    End Sub

    Private Sub btnSetSeasonPosterScrape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonPosterScrape.Click
        Dim tImage As MediaContainers.Image = ModulesManager.Instance.TVSingleImageOnly(Me.tmpDBTVSeason.TVShow.Title, Convert.ToInt32(Me.tmpDBTVSeason.ShowID), Me.tmpDBTVSeason.TVShow.TVDB, Enums.ModifierType.SeasonPoster, Me.tmpDBTVSeason.TVSeason.Season, 0, Me.tmpDBTVSeason.Language, Me.tmpDBTVSeason.Ordering, CType(Me.tmpDBTVSeason.ImagesContainer.Poster, MediaContainers.Image))

        If tImage IsNot Nothing AndAlso tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Poster = tImage
            Me.pbSeasonPoster.Image = tImage.WebImage.Image
            Me.pbSeasonPoster.Tag = tImage

            Me.lblSeasonPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonPoster.Image.Width, Me.pbSeasonPoster.Image.Height)
            Me.lblSeasonPosterSize.Visible = True
        End If
    End Sub

    Private Sub btnSetSeasonPosterLocal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetSeasonPosterLocal.Click
        With ofdImage
            .InitialDirectory = Me.tmpDBTVSeason.ShowPath
            .Filter = Master.eLang.GetString(497, "Images") + "|*.jpg;*.png"
            .FilterIndex = 0
        End With

        If ofdImage.ShowDialog() = DialogResult.OK Then
            Dim tImage As New MediaContainers.Image
            tImage.WebImage.FromFile(ofdImage.FileName)
            If tImage.WebImage.Image IsNot Nothing Then
                Me.tmpDBTVSeason.ImagesContainer.Poster = tImage
                Me.pbSeasonPoster.Image = tImage.WebImage.Image
                Me.pbSeasonPoster.Tag = tImage

                Me.lblSeasonPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonPoster.Image.Width, Me.pbSeasonPoster.Image.Height)
                Me.lblSeasonPosterSize.Visible = True
            End If
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dlgEditSeason_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.tmpDBTVSeason.IsOnline OrElse FileUtils.Common.CheckOnlineStatus_TVShow(Me.tmpDBTVSeason, True) Then
            If Not Master.eSettings.TVSeasonBannerAnyEnabled Then tcEditSeason.TabPages.Remove(tpSeasonBanner)
            If Not Master.eSettings.TVSeasonFanartAnyEnabled Then tcEditSeason.TabPages.Remove(tpSeasonFanart)
            If Not Master.eSettings.TVSeasonLandscapeAnyEnabled Then tcEditSeason.TabPages.Remove(tpSeasonLandscape)
            If Not Master.eSettings.TVSeasonPosterAnyEnabled Then tcEditSeason.TabPages.Remove(tpSeasonPoster)

            Me.pbSeasonBanner.AllowDrop = True
            Me.pbSeasonFanart.AllowDrop = True
            Me.pbSeasonLandscape.AllowDrop = True
            Me.pbSeasonPoster.AllowDrop = True

            Me.SetUp()

            Dim iBackground As New Bitmap(Me.pnlTop.Width, Me.pnlTop.Height)
            Using g As Graphics = Graphics.FromImage(iBackground)
                g.FillRectangle(New Drawing2D.LinearGradientBrush(Me.pnlTop.ClientRectangle, Color.SteelBlue, Color.LightSteelBlue, Drawing2D.LinearGradientMode.Horizontal), pnlTop.ClientRectangle)
                Me.pnlTop.BackgroundImage = iBackground
            End Using

            Me.FillInfo()
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Sub FillInfo()
        With Me
            If Master.eSettings.TVSeasonBannerAnyEnabled Then
                If Me.tmpDBTVSeason.ImagesContainer.Banner.WebImage.Image IsNot Nothing Then
                    .pbSeasonBanner.Image = Me.tmpDBTVSeason.ImagesContainer.Banner.WebImage.Image
                    .pbSeasonBanner.Tag = Me.tmpDBTVSeason.ImagesContainer.Banner

                    .lblSeasonBannerSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), .pbSeasonBanner.Image.Width, .pbSeasonBanner.Image.Height)
                    .lblSeasonBannerSize.Visible = True
                End If
            End If

            If Master.eSettings.TVSeasonFanartAnyEnabled Then
                If Me.tmpDBTVSeason.ImagesContainer.Fanart.WebImage.Image IsNot Nothing Then
                    .pbSeasonFanart.Image = Me.tmpDBTVSeason.ImagesContainer.Fanart.WebImage.Image
                    .pbSeasonFanart.Tag = Me.tmpDBTVSeason.ImagesContainer.Fanart

                    .lblSeasonFanartSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), .pbSeasonFanart.Image.Width, .pbSeasonFanart.Image.Height)
                    .lblSeasonFanartSize.Visible = True
                End If
            End If

            If Master.eSettings.TVSeasonLandscapeAnyEnabled Then
                If Me.tmpDBTVSeason.ImagesContainer.Landscape.WebImage.Image IsNot Nothing Then
                    .pbSeasonLandscape.Image = Me.tmpDBTVSeason.ImagesContainer.Landscape.WebImage.Image
                    .pbSeasonLandscape.Tag = Me.tmpDBTVSeason.ImagesContainer.Landscape

                    .lblSeasonLandscapeSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), .pbSeasonLandscape.Image.Width, .pbSeasonLandscape.Image.Height)
                    .lblSeasonLandscapeSize.Visible = True
                End If
            End If

            If Master.eSettings.TVSeasonPosterAnyEnabled Then
                If Me.tmpDBTVSeason.ImagesContainer.Poster.WebImage.Image IsNot Nothing Then
                    .pbSeasonPoster.Image = Me.tmpDBTVSeason.ImagesContainer.Poster.WebImage.Image
                    .pbSeasonPoster.Tag = Me.tmpDBTVSeason.ImagesContainer.Poster

                    .lblSeasonPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), .pbSeasonPoster.Image.Width, .pbSeasonPoster.Image.Height)
                    .lblSeasonPosterSize.Visible = True
                End If
            End If
        End With
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Master.DB.SaveTVSeasonToDB(Me.tmpDBTVSeason, False)

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub pbSeasonBanner_DragDrop(sender As Object, e As DragEventArgs) Handles pbSeasonBanner.DragDrop
        Dim tImage As MediaContainers.Image = FileUtils.DragAndDrop.GetDoppedImage(e)
        If tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Banner = tImage
            Me.pbSeasonBanner.Image = tImage.WebImage.Image
            Me.pbSeasonBanner.Tag = tImage
            Me.lblSeasonBannerSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonBanner.Image.Width, Me.pbSeasonBanner.Image.Height)
            Me.lblSeasonBannerSize.Visible = True
        End If
    End Sub

    Private Sub pbSeasonBanner_DragEnter(sender As Object, e As DragEventArgs) Handles pbSeasonBanner.DragEnter
        If FileUtils.DragAndDrop.CheckDroppedImage(e) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub pbSeasonFanart_DragDrop(sender As Object, e As DragEventArgs) Handles pbSeasonFanart.DragDrop
        Dim tImage As MediaContainers.Image = FileUtils.DragAndDrop.GetDoppedImage(e)
        If tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Fanart = tImage
            Me.pbSeasonFanart.Image = tImage.WebImage.Image
            Me.pbSeasonFanart.Tag = tImage
            Me.lblSeasonFanartSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonFanart.Image.Width, Me.pbSeasonFanart.Image.Height)
            Me.lblSeasonFanartSize.Visible = True
        End If
    End Sub

    Private Sub pbSeasonFanart_DragEnter(sender As Object, e As DragEventArgs) Handles pbSeasonFanart.DragEnter
        If FileUtils.DragAndDrop.CheckDroppedImage(e) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub pbSeasonLandscape_DragDrop(sender As Object, e As DragEventArgs) Handles pbSeasonLandscape.DragDrop
        Dim tImage As MediaContainers.Image = FileUtils.DragAndDrop.GetDoppedImage(e)
        If tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Landscape = tImage
            Me.pbSeasonLandscape.Image = tImage.WebImage.Image
            Me.pbSeasonLandscape.Tag = tImage
            Me.lblSeasonLandscapeSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonLandscape.Image.Width, Me.pbSeasonLandscape.Image.Height)
            Me.lblSeasonLandscapeSize.Visible = True
        End If
    End Sub

    Private Sub pbSeasonLandscape_DragEnter(sender As Object, e As DragEventArgs) Handles pbSeasonLandscape.DragEnter
        If FileUtils.DragAndDrop.CheckDroppedImage(e) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub pbSeasonPoster_DragDrop(sender As Object, e As DragEventArgs) Handles pbSeasonPoster.DragDrop
        Dim tImage As MediaContainers.Image = FileUtils.DragAndDrop.GetDoppedImage(e)
        If tImage.WebImage.Image IsNot Nothing Then
            Me.tmpDBTVSeason.ImagesContainer.Poster = tImage
            Me.pbSeasonPoster.Image = tImage.WebImage.Image
            Me.pbSeasonPoster.Tag = tImage
            Me.lblSeasonPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), Me.pbSeasonPoster.Image.Width, Me.pbSeasonPoster.Image.Height)
            Me.lblSeasonPosterSize.Visible = True
        End If
    End Sub

    Private Sub pbSeasonPoster_DragEnter(sender As Object, e As DragEventArgs) Handles pbSeasonPoster.DragEnter
        If FileUtils.DragAndDrop.CheckDroppedImage(e) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub SetUp()
        Me.Text = Master.eLang.GetString(769, "Edit Season")
        Me.Cancel_Button.Text = Master.eLang.GetString(167, "Cancel")
        Me.OK_Button.Text = Master.eLang.GetString(179, "OK")
        Me.btnRemoveSeasonBanner.Text = Master.eLang.GetString(1024, "Remove Banner")
        Me.btnRemoveSeasonFanart.Text = Master.eLang.GetString(250, "Remove Fanart")
        Me.btnRemoveSeasonLandscape.Text = Master.eLang.GetString(1034, "Remove Landscape")
        Me.btnRemoveSeasonPoster.Text = Master.eLang.GetString(247, "Remove Poster")
        Me.btnSetSeasonBannerDL.Text = Master.eLang.GetString(1023, "Change Banner (Download)")
        Me.btnSetSeasonBannerLocal.Text = Master.eLang.GetString(1021, "Change Banner (Local)")
        Me.btnSetSeasonBannerScrape.Text = Master.eLang.GetString(1022, "Change Banner (Scrape)")
        Me.btnSetSeasonFanartDL.Text = Master.eLang.GetString(266, "Change Fanart (Download)")
        Me.btnSetSeasonFanartLocal.Text = Master.eLang.GetString(252, "Change Fanart (Local)")
        Me.btnSetSeasonFanartScrape.Text = Master.eLang.GetString(251, "Change Fanart (Scrape)")
        Me.btnSetSeasonLandscapeDL.Text = Master.eLang.GetString(1033, "Change Landscape (Download)")
        Me.btnSetSeasonLandscapeLocal.Text = Master.eLang.GetString(1031, "Change Landscape (Local)/")
        Me.btnSetSeasonLandscapeScrape.Text = Master.eLang.GetString(1032, "Change Landscape (Scrape)")
        Me.btnSetSeasonPosterDL.Text = Master.eLang.GetString(265, "Change Poster (Download)")
        Me.btnSetSeasonPosterLocal.Text = Master.eLang.GetString(249, "Change Poster (Local)")
        Me.btnSetSeasonPosterScrape.Text = Master.eLang.GetString(248, "Change Poster (Scrape)")
        Me.lblTopTitle.Text = Me.Text
        Me.tblTopDetails.Text = Master.eLang.GetString(830, "Edit the details for the selected season.")
        Me.tpSeasonBanner.Text = Master.eLang.GetString(838, "Banner")
        Me.tpSeasonFanart.Text = Master.eLang.GetString(149, "Fanart")
        Me.tpSeasonLandscape.Text = Master.eLang.GetString(1035, "Landscape")
        Me.tpSeasonPoster.Text = Master.eLang.GetString(148, "Poster")
    End Sub

#End Region 'Methods

End Class