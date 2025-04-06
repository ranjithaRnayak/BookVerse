import { useState, React } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { Routes, Route } from 'react-router-dom';
import Login from './pages/Login';
import BookList from './pages/BookList';
import AddBook from './pages/AddBook';
function App() {
  const [count, setCount] = useState(0)

  return (
    <>
     <div className="p-6">
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/books" element={<BookList />} />
        <Route path="/add" element={<AddBook />} />
      </Routes>
    </div>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
        <div className="text-3xl font-bold underline text-blue-600">
      Hello BookVerse!
    </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.jsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
