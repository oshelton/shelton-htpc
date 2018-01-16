//Copyright (c) 2017 Owen Shelton
//Licensed under the MIT license.
//See License file for licensing details.

#include <QtCore/QCoreApplication>
#include <QtCore/qjsondocument.h>
#include <QtCore/qjsonobject.h>
#include <QtCore/qjsonarray.h>
#include <QtCore/qfile.h>
#include <QtCore/qdebug.h>
#include <iostream>

#include "main.h"

ApplicationSettingsModel::ApplicationSettingsModel(const QJsonDocument& doc)
{
	auto root = doc.object();
	if (root.contains("ApplicationSettings"))
	{
		QJsonObject applicationSettingsNode = root["ApplicationSettings"].toObject();
		_runOnStartup = applicationSettingsNode.contains("RunOnStartup") ?
			applicationSettingsNode["RunOnStartup"].toBool() : false;
		_idleWaitTime = applicationSettingsNode.contains("IdleWaitTime") ?
			applicationSettingsNode["IdleWaitTime"].toInt() : 10;

		_enableMovies = applicationSettingsNode.contains("EnableMovies") ?
			applicationSettingsNode["EnableMovies"].toBool() : false;
		_enableSeries = applicationSettingsNode.contains("EnableSeries") ?
			applicationSettingsNode["EnableSeries"].toBool() : false;
		_enableMusic = applicationSettingsNode.contains("EnableMusic") ?
			applicationSettingsNode["EnableMusic"].toBool() : false;
		_enablePictures = applicationSettingsNode.contains("EnablePictures") ?
			applicationSettingsNode["EnablePictures"].toBool() : false;
		_enableGames = applicationSettingsNode.contains("EnableGames") ?
			applicationSettingsNode["EnableGames"].toBool() : false;

		if (applicationSettingsNode.contains("MovieSourceDirectories"))
		{
			QJsonArray dirArray = applicationSettingsNode["MovieSourceDirectories"].toArray();
			for (auto item : dirArray)
			{
				_movieSourceDirectories.append(item.toString());
			}
		}

		Q_ASSERT(_runOnStartup == true);
		Q_ASSERT(_idleWaitTime == 10);
		Q_ASSERT(_enableMovies == true);
		Q_ASSERT(_enableSeries == true);
		Q_ASSERT(_enableMusic == false);
		Q_ASSERT(_enablePictures == true);
		Q_ASSERT(_enableGames == true);

		Q_ASSERT(_movieSourceDirectories.size() == 2);
		Q_ASSERT(_movieSourceDirectories[0] == "C:\\tmp1");
	}
}

int main(int argc, char *argv[])
{
	QFile jsonFile("data.json");
	jsonFile.open(QFile::ReadOnly);
	ApplicationSettingsModel settings(QJsonDocument::fromJson(jsonFile.readAll()));

	qDebug() << "Run on startup:" << settings.runOnStartup();
	qDebug() << "Idle wait time:" << settings.idleWaitTime();
	qDebug() << "Enable movies:" << settings.enableMovies();
	qDebug() << "Enable series:" << settings.enableSeries();
	qDebug() << "Enable music:" << settings.enableMusic();
	qDebug() << "Enable pictures:" << settings.enablePictures();
	qDebug() << "Enable games:" << settings.enableGames();

	for (unsigned int i = 0; i < settings.movieSourceDirectories().count(); ++i)
	{
		qDebug() << "Movie source directory" << i << ":" << settings.movieSourceDirectories()[i];
	}

	std::cin.get();
}
