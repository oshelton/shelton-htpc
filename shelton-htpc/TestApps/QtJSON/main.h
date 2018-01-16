//Copyright (c) 2017 Owen Shelton
//Licensed under the MIT license.
//See License file for licensing details.

#pragma once

#include <QtCore\qobject.h>

class ApplicationSettingsModel : public QObject
{
	Q_OBJECT

	Q_PROPERTY(bool runOnStartup READ runOnStartup)
	Q_PROPERTY(int idleWaitTime READ idleWaitTime)
	Q_PROPERTY(bool enableMovies READ enableMovies)
	Q_PROPERTY(bool enableSeries READ enableSeries)
	Q_PROPERTY(bool enableMusic READ enableMusic)
	Q_PROPERTY(bool enablePictures READ enablePictures)
	Q_PROPERTY(bool enableGames READ enableGames)

	Q_PROPERTY(QList<QString> movieSourceDirectories READ movieSourceDirectories)

private:
	bool _runOnStartup;
	int _idleWaitTime;
	bool _enableMovies;
	bool _enableSeries;
	bool _enableMusic;
	bool _enablePictures;
	bool _enableGames;

	QList<QString> _movieSourceDirectories;

public:
	ApplicationSettingsModel(const QJsonDocument& doc);

	const bool& runOnStartup() { return _runOnStartup; }
	const int& idleWaitTime() { return _idleWaitTime; }
	const bool& enableMovies() { return _enableMovies; }
	const bool& enableSeries() { return _enableSeries; }
	const bool& enableMusic() { return _enableMusic; }
	const bool& enablePictures() { return _enablePictures; }
	const bool& enableGames() { return _enableMovies; }
	const QList<QString>& movieSourceDirectories() { return _movieSourceDirectories; }
};