import { Button, Card, Space } from "antd";
import { Routes, Route, useNavigate } from "react-router-dom";
import TodoGroupList from "../features/TodoGroup/TodoGroupList";
import TodoGroupCreate from "../features/TodoGroup/TodoGroupCreate";
import { TodoGroupEdit } from "../features/TodoGroup/TodoGroupEdit";
import { TodoGroupView } from "../features/TodoGroup/TodoGroupView";

export const TodoGroupPage = () => {
  const navigate = useNavigate();
  return (
    <Card>
      <h1>Manage Todo Lists</h1>

      <Routes>
        <Route
          path="/"
          element={
            <>
              <Space style={{ marginBottom: 16 }}>
                <Button type="primary" onClick={() => navigate("create")}>
                  Create Todo List
                </Button>
              </Space>
              <TodoGroupList />
            </>
          }
        />

        <Route
          path="create"
          element={
            <>
              <TodoGroupCreate
                visible={true}
                onClose={() => navigate("/todo-list")}
              />
              <TodoGroupList />
            </>
          }
        />
        <Route path="edit/:id" element={<TodoGroupEdit />} />
        <Route path="view/:id" element={<TodoGroupView />} />
      </Routes>
    </Card>
  );
};
