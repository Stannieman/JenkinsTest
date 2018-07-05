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
    stage('Build') {
      steps {
	      powershell 'ls env:'
	      powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" build -c Release"'
      }
    }
	stage('Test') {
		steps {
			powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" vstest --parallel --logger:trx ((ls -Recurse *.UnitTests.dll | % FullName) -Match `"\\\\bin\\\\Release\\\\`")"'
		}
		post {
			always {
				step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: true, keepLongStdio: true])
			}
		}
	}
	stage('Pack') {
		steps {
			powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" pack -c Release --include-source --include-symbols --no-restore --no-build"'
		}
	}
	stage('Archive') {
		steps {
			archiveArtifacts artifacts: '**/**/**/*.nupkg', fingerprint: true
		}
	}
	  stage('Publish') {
		  when {
			  expression { params.PUBLISH && env.GIT_BRANCH.startsWith('origin/mast') }
		  }
		  steps {
			  echo "Publishing!"
		  }
	  }
  }
	  post {
		  cleanup {
			  cleanWs()
		  }
	  }
}
