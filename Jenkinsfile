pipeline {
  agent {
    label 'Git'
  }
	parameters {
		booleanParam(
			defaultValue: false,
			name: 'PUBLISH',
			description: 'If true then the artifacts will be pushed to NuGet.org')
	}
  stages {
	  stage('Set version') {
		  steps {
			  bat 'pwsh.exe -File ./Scripts/UpdateVersion.ps1'
		  }
	  }
	  stage('Restore packages') {
		  steps {
			  bat '"%EXECUTABLE_DOTNET_CORE_2%" restore'
		  }
	  }
    stage('Build') {
      steps {
	      bat '"%EXECUTABLE_DOTNET_CORE_2%" build -c Release --no-restore'
      }
    }
	stage('Test') {
		steps {
			bat 'pwsh.exe -Command "& \'%EXECUTABLE_DOTNET_CORE_2%\' vstest ((Get-ChildItem -Recurse *.UnitTests.dll | Select-Object -ExpandProperty FullName) -Match \'\\\\bin\\\\Release\\\\\') --parallel --logger:trx"'
		}
		post {
			always {
				step([$class: 'MSTestPublisher', testResultsFile: '**/*.trx', failOnError: true, keepLongStdio: true])
			}
		}
	}
	stage('Pack') {
		when {
			  expression { params.PUBLISH || env.GIT_BRANCH == 'origin/develop' }
		  }
		steps {
			bat '"%EXECUTABLE_DOTNET_CORE_2%" pack -c Release --include-source --include-symbols --no-restore --no-build'
			archiveArtifacts artifacts: '**/*.nupkg', fingerprint: true
		}
	}
	  stage('Publish') {
		  when {
			  expression { params.PUBLISH || env.GIT_BRANCH.startsWith('origin/release') }
		  }
		  steps {
			  bat 'pwsh.exe -Command "& \'%EXECUTABLE_DOTNET_CORE_2%\' nuget push -s https://staging.nuget.org -ss https://nuget.smbsrc.net -k %NUGET_ORG_API_KEY% ((Get-ChildItem -Recurse *.nupkg | Select-Object -ExpandProperty FullName) -Match \'\\\\bin\\\\Release\\\\\')"'
		  }
	  }
  }
	  /*post {
		  cleanup {
			  cleanWs()
		  }
	  }*/
}
