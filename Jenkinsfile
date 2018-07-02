pipeline {
  agent {
    label 'Git'
  }
  stages {
    stage('Build') {
      steps {
		powershell 'Invoke-Expression "$env:CAPABILITY_MSBUILD_15.7 /t:Build /p:Configuration=Release"'
      }
    }
	stage('Test') {
		steps {
			powershell 'Write-Host "Teeest"'
		}
	}
	stage('Pack') {
		steps {
			powershell 'Invoke-Expression "$env:CAPABILITY_MSBUILD_15.7 /t:pack /p:Configuration=Release"'
		}
	}
	stage('Publish') {
		steps {
			echo "Publish"
		}
	}
  }
}
