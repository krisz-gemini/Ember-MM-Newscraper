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
Imports NLog


Public Class CommandLine

#Region "Fields"

    Shared logger As Logger = LogManager.GetCurrentClassLogger()

#End Region 'Fields

#Region "Events"

    Public Event TaskEvent(ByVal mType As Enums.ModuleEventType, ByRef _params As List(Of Object))

    'Singleton Instace for CommandLine manager .. allways use this one
    Private Shared Singleton As CommandLine = Nothing

#End Region 'Events

#Region "Properties"

    Public Shared ReadOnly Property Instance() As CommandLine
        Get
            If (Singleton Is Nothing) Then
                Singleton = New CommandLine()
            End If
            Return Singleton
        End Get
    End Property

#End Region 'Properties

#Region "Methods"

    Public Sub RunCommandLine(ByVal args() As String)
        If args.Count = 0 Then Return

        logger.Trace("Call CommandLine")

        Dim MoviePath As String = String.Empty
        Dim isSingle As Boolean = False
        Dim clExport As Boolean = False
        Dim clExportResizePoster As Integer = 0
        Dim clExportTemplate As String = "template"
        Dim nowindow As Boolean = False
        Dim RunModule As Boolean = False
        Dim ModuleName As String = String.Empty

        For i As Integer = 0 To args.Count - 1

            Select Case args(i).ToLower
                Case "-addmoviesource"
                    If args.Count - 1 > i Then
                        If Directory.Exists(args(i + 1).Replace("""", String.Empty)) Then
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"addmoviesource", args(i + 1).Replace("""", String.Empty)}))
                            i += 1
                        End If
                    Else
                        logger.Warn("[CommandLine] No path or invalid path specified for -addmoviesource command")
                    End If
                Case "-addtvshowsource"
                    If args.Count - 1 > i Then
                        If Directory.Exists(args(i + 1).Replace("""", String.Empty)) Then
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"addtvshowsource", args(i + 1).Replace("""", String.Empty)}))
                            i += 1
                        End If
                    Else
                        logger.Warn("[CommandLine] No path or invalid path specified for -addtvshowsource command")
                    End If
                Case "-cleanvideodb"
                    RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"cleanvideodb"}))
                Case "-close"
                    RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"close"}))
                Case "-nosplash"
                    Master.fLoading.Hide()
                Case "-profile"
                    'has been handled in ApplicationEvents.vb
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        'skip profile name
                        i += 1
                    End If
                Case "-run"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        Dim strModuleName As String = args(i + 1)
                        i += 1
                        Dim sParams As List(Of Object) = Nothing
                        i = SetModuleParameters(args, i, sParams)
                        RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"run", strModuleName, sParams}))
                    Else
                        logger.Warn("[CommandLine] Missing module name for command ""-run""")
                    End If
                Case "-scanfolder"
                    If args.Count - 1 > i Then
                        If Directory.Exists(args(i + 1).Replace("""", String.Empty)) Then
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.SpecificFolder = True}, -1, args(i + 1).Replace("""", String.Empty)}))
                            i += 1
                        End If
                    Else
                        logger.Warn("[CommandLine] No path or invalid path specified for command ""-scanfolder""")
                    End If
                Case "-scrapemovies"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        i += 1
                        Dim ScrapeType = SetScrapeType(args(i))
                        If Not ScrapeType = Enums.ScrapeType.None Then
                            Dim CustomScrapeModifiers As New Structures.ScrapeModifiers
                            i = SetScraperMod(args, i, CustomScrapeModifiers)
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"scrapemovies", ScrapeType, CustomScrapeModifiers}))
                        Else
                            logger.Warn(String.Format("[CommandLine] Invalid ScrapeType specified for command ""-scrapemovies"": {0}", args(i)))
                        End If
                    Else
                        logger.Warn("[CommandLine] No ScrapeType specified for command ""-scrapemovies""")
                    End If
                Case "-scrapemoviesets"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        i += 1
                        Dim ScrapeType = SetScrapeType(args(i))
                        If Not ScrapeType = Enums.ScrapeType.None Then
                            Dim CustomScrapeModifiers As New Structures.ScrapeModifiers
                            i = SetScraperMod(args, i, CustomScrapeModifiers)
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"scrapemoviesets", ScrapeType, CustomScrapeModifiers}))
                        Else
                            logger.Warn(String.Format("[CommandLine] Invalid ScrapeType specified for command ""-scrapemoviesets"": {0}", args(i)))
                        End If
                    Else
                        logger.Warn("[CommandLine] No ScrapeType specified for command ""-scrapemoviesets""")
                    End If
                Case "-scrapetvshows"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        i += 1
                        Dim ScrapeType = SetScrapeType(args(i))
                        If Not ScrapeType = Enums.ScrapeType.None Then
                            Dim CustomScrapeModifiers As New Structures.ScrapeModifiers
                            i = SetScraperMod(args, i, CustomScrapeModifiers)
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine, New List(Of Object)(New Object() {"scrapetvshows", ScrapeType, CustomScrapeModifiers}))
                        Else
                            logger.Warn(String.Format("[CommandLine] Invalid ScrapeType specified for command ""-scrapetvshows"": {0}", args(i)))
                        End If
                    Else
                        logger.Warn("[CommandLine] No ScrapeType specified for command ""-scrapetvshows""")
                    End If
                Case "-updatemovies"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        Dim clArg As String = args(i + 1).Replace("""", String.Empty)
                        Dim sSource As Database.DBSource = Master.DB.GetSources_Movie.FirstOrDefault(Function(f) f.Name.ToLower = clArg.ToLower)
                        If sSource IsNot Nothing Then
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.Movies = True}, sSource.ID, String.Empty}))
                            i += 1
                        Else
                            sSource = Master.DB.GetSources_Movie.FirstOrDefault(Function(f) f.Path.ToLower = clArg.ToLower)
                            If sSource IsNot Nothing Then
                                RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                        New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.Movies = True}, sSource.ID, String.Empty}))
                                i += 1
                            Else
                                RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.Movies = True}, -1, String.Empty}))
                            End If
                        End If
                    Else
                        RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.Movies = True}, -1, String.Empty}))
                    End If
                Case "-updatetvshows"
                    If args.Count - 1 > i AndAlso Not args(i + 1).StartsWith("-") Then
                        Dim clArg As String = args(i + 1).Replace("""", String.Empty)
                        Dim sSource As Database.DBSource = Master.DB.GetSources_TVShow.FirstOrDefault(Function(f) f.Name.ToLower = clArg.ToLower)
                        If sSource IsNot Nothing Then
                            RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.TV = True}, sSource.ID, String.Empty}))
                            i += 1
                        Else
                            sSource = Master.DB.GetSources_TVShow.FirstOrDefault(Function(f) f.Path.ToLower = clArg.ToLower)
                            If sSource IsNot Nothing Then
                                RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                        New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.TV = True}, sSource.ID, String.Empty}))
                                i += 1
                            Else
                                RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.TV = True}, -1, String.Empty}))
                            End If
                        End If
                    Else
                        RaiseEvent TaskEvent(Enums.ModuleEventType.CommandLine,
                                                    New List(Of Object)(New Object() {"loadmedia", New Structures.ScanOrClean With {.TV = True}, -1, String.Empty}))
                    End If
                Case Else
                    logger.Warn(String.Concat("[CommandLine] Invalid command: ", args(i)))
            End Select
        Next
    End Sub

    Private Function SetModuleParameters(ByVal args() As String, ByVal startPos As Integer, ByRef parameters As List(Of Object)) As Integer
        Dim iEndPos As Integer = startPos

        For i As Integer = startPos + 1 To args.Count - 1
            If Not args(i).StartsWith("-") Then
                If parameters Is Nothing Then parameters = New List(Of Object)
                parameters.Add(args(i))
            Else
                Return i - 1
            End If
            iEndPos = i
        Next

        Return iEndPos
    End Function

    Private Function SetScraperMod(ByVal args() As String, ByVal startPos As Integer, ByRef scrapeModifiers As Structures.ScrapeModifiers) As Integer
        Dim iEndPos As Integer = startPos

        For i As Integer = startPos + 1 To args.Count - 1
            Select Case args(i).ToLower
                Case "all"
                    scrapeModifiers.AllSeasonsBanner = True
                    scrapeModifiers.AllSeasonsFanart = True
                    scrapeModifiers.AllSeasonsLandscape = True
                    scrapeModifiers.AllSeasonsPoster = True
                    scrapeModifiers.EpisodeActorThumbs = True
                    scrapeModifiers.EpisodeFanart = True
                    scrapeModifiers.EpisodeMeta = True
                    scrapeModifiers.EpisodeNFO = True
                    scrapeModifiers.EpisodePoster = True
                    scrapeModifiers.MainActorthumbs = True
                    scrapeModifiers.MainBanner = True
                    scrapeModifiers.MainCharacterArt = True
                    scrapeModifiers.MainClearArt = True
                    scrapeModifiers.MainClearLogo = True
                    scrapeModifiers.MainDiscArt = True
                    scrapeModifiers.MainExtrafanarts = True
                    scrapeModifiers.MainExtrathumbs = True
                    scrapeModifiers.MainFanart = True
                    scrapeModifiers.MainLandscape = True
                    scrapeModifiers.MainMeta = True
                    scrapeModifiers.MainNFO = True
                    scrapeModifiers.MainPoster = True
                    scrapeModifiers.MainSubtitles = True
                    scrapeModifiers.MainTheme = True
                    scrapeModifiers.MainTrailer = True
                    scrapeModifiers.SeasonBanner = True
                    scrapeModifiers.SeasonFanart = True
                    scrapeModifiers.SeasonLandscape = True
                    scrapeModifiers.SeasonNFO = True
                    scrapeModifiers.SeasonPoster = True
                    scrapeModifiers.withEpisodes = True
                    scrapeModifiers.withSeasons = True
                Case "episodeall"
                    scrapeModifiers.EpisodeActorThumbs = True
                    scrapeModifiers.EpisodeFanart = True
                    scrapeModifiers.EpisodeMeta = True
                    scrapeModifiers.EpisodeNFO = True
                    scrapeModifiers.EpisodePoster = True
                    scrapeModifiers.withEpisodes = True
                Case "episodeactorthumbs"
                    scrapeModifiers.EpisodeActorThumbs = True
                    scrapeModifiers.withEpisodes = True
                Case "episodefanart"
                    scrapeModifiers.EpisodeFanart = True
                    scrapeModifiers.withEpisodes = True
                Case "episodemeta"
                    scrapeModifiers.EpisodeMeta = True
                    scrapeModifiers.withEpisodes = True
                Case "episodenfo"
                    scrapeModifiers.EpisodeNFO = True
                    scrapeModifiers.withEpisodes = True
                Case "episodeposter"
                    scrapeModifiers.EpisodePoster = True
                    scrapeModifiers.withEpisodes = True
                Case "actorthumbs"
                    scrapeModifiers.MainActorthumbs = True
                Case "banner"
                    scrapeModifiers.MainBanner = True
                Case "characterart"
                    scrapeModifiers.MainCharacterArt = True
                Case "clearart"
                    scrapeModifiers.MainClearArt = True
                Case "clearlogo"
                    scrapeModifiers.MainClearLogo = True
                Case "discart"
                    scrapeModifiers.MainDiscArt = True
                Case "extrafanarts"
                    scrapeModifiers.MainExtrafanarts = True
                Case "extrathumbs"
                    scrapeModifiers.MainExtrathumbs = True
                Case "fanart"
                    scrapeModifiers.MainFanart = True
                Case "landscape"
                    scrapeModifiers.MainLandscape = True
                Case "meta"
                    scrapeModifiers.MainMeta = True
                Case "nfo"
                    scrapeModifiers.MainNFO = True
                Case "poster"
                    scrapeModifiers.MainPoster = True
                Case "subtitles"
                    scrapeModifiers.MainSubtitles = True
                Case "theme"
                    scrapeModifiers.MainTheme = True
                Case "trailer"
                    scrapeModifiers.MainTrailer = True
                Case "seasonall"
                    scrapeModifiers.SeasonBanner = True
                    scrapeModifiers.SeasonFanart = True
                    scrapeModifiers.SeasonLandscape = True
                    scrapeModifiers.SeasonNFO = True
                    scrapeModifiers.SeasonPoster = True
                    scrapeModifiers.withSeasons = True
                Case "seasonbanner"
                    scrapeModifiers.SeasonBanner = True
                    scrapeModifiers.withSeasons = True
                Case "seasonfanart"
                    scrapeModifiers.SeasonFanart = True
                    scrapeModifiers.withSeasons = True
                Case "seasonlandscape"
                    scrapeModifiers.SeasonLandscape = True
                    scrapeModifiers.withSeasons = True
                Case "seasonnfo"
                    scrapeModifiers.SeasonNFO = True
                    scrapeModifiers.withSeasons = True
                Case "seasonposter"
                    scrapeModifiers.SeasonPoster = True
                    scrapeModifiers.withSeasons = True
                Case Else
                    Return i - 1
            End Select
            iEndPos = i
        Next

        Return iEndPos
    End Function

    Private Function SetScrapeType(ByVal scrapetype As String) As Enums.ScrapeType
        Select Case scrapetype
            Case "allask"
                Return Enums.ScrapeType.AllAsk
            Case "allauto"
                Return Enums.ScrapeType.AllAuto
            Case "allskip"
                Return Enums.ScrapeType.AllSkip
            Case "markedask"
                Return Enums.ScrapeType.MarkedAsk
            Case "markedauto"
                Return Enums.ScrapeType.MarkedAuto
            Case "markedskip"
                Return Enums.ScrapeType.MarkedSkip
            Case "missingask"
                Return Enums.ScrapeType.MissingAsk
            Case "missingauto"
                Return Enums.ScrapeType.MissingAuto
            Case "missingskip"
                Return Enums.ScrapeType.MissingSkip
            Case "newask"
                Return Enums.ScrapeType.NewAsk
            Case "newauto"
                Return Enums.ScrapeType.NewAuto
            Case "newskip"
                Return Enums.ScrapeType.NewSkip
            Case Else
                Return Enums.ScrapeType.None
        End Select
    End Function

#End Region 'Methods

End Class
