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
	      bat '"%EXECUTABLE_DOTNET_CORE_2_0%" build -c Release'
      }
    }
	stage('Test') {
		steps {
			bat 'pwsh.exe -Command Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_CORE_2_0}`" vstest --parallel --logger:trx ((ls -Recurse *.UnitTests.dll | % FullName) -Match `"\\\\bin\\\\Release\\\\`")"'
		}
		post {
			always {
				step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: true, keepLongStdio: true])
			}
		}
	}
	stage('Pack') {
		when {
			  expression { env.GIT_BRANCH == 'origin/develop' }
		  }
		steps {
			bat '"%EXECUTABLE_DOTNET_CORE_2_0}" pack -c Release --include-source --include-symbols --no-restore --no-build'
			archiveArtifacts artifacts: '**/**/**/*.nupkg', fingerprint: true
		}
	}
	  stage('Publish') {
		  when {
			  expression { params.PUBLISH && env.GIT_BRANCH.startsWith('origin/release') }
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
