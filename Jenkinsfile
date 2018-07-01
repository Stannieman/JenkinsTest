pipeline {
  agent {
    label 'Git'
  }
  stages {
    stage('Build') {
      steps {
        powershell "mkdir testdir"
        cleanWs()
      }
    }
  }
}
