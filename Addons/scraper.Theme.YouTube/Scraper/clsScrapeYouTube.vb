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

Imports EmberAPI

Public Class Scraper

#Region "Methods"

    Public Function GetThemes(ByVal title As String, ByVal type As Enums.ContentType) As List(Of MediaContainers.MediaFile)
        Dim lstThemes As New List(Of MediaContainers.MediaFile)
        Dim strDefaultSearch As String = String.Empty
        Select Case type
            Case Enums.ContentType.Movie
                strDefaultSearch = Master.eSettings.MovieThemeDefaultSearch
            Case Enums.ContentType.TV, Enums.ContentType.TVShow
                strDefaultSearch = Master.eSettings.TVShowThemeDefaultSearch
        End Select
        lstThemes = YouTube.Scraper.SearchOnYouTube(String.Format("{0} {1}", title, strDefaultSearch))
        For Each tTheme In lstThemes
            tTheme.Scraper = "YouTube"
        Next
        Return lstThemes
    End Function

#End Region 'Methods

End Class